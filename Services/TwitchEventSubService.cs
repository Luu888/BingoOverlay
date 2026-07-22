using BingoOverlay.Data;
using BingoOverlay.Models;
using BingoOverlay.Models.Enums;
using BingoOverlay.Services;
using Microsoft.Extensions.Options;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace BingoOverlay.Services;

public class TwitchEventSubService : BackgroundService
{
    private readonly TwitchOptions _options;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IServiceScopeFactory _scopeFactory;

    public TwitchEventSubService(
        IOptions<TwitchOptions> options,
        IHttpClientFactory httpClientFactory,
        IServiceScopeFactory scopeFactory)
    {
        _options = options.Value;
        _httpClientFactory = httpClientFactory;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (!await IsTwitchAuthorizedAsync())
            {
                Console.WriteLine("Twitch nie jest jeszcze podłączony");
                await Task.Delay(TimeSpan.FromSeconds(5),stoppingToken);

                continue;
            }

            await RunEventSubAsync(stoppingToken);
        }
    }

    private async Task HandleNotificationAsync(JsonDocument document, CancellationToken cancellationToken)
    {
        var root = document.RootElement;

        var subscriptionType =
            root.GetProperty("metadata")
            .GetProperty("subscription_type")
            .GetString();

        if (subscriptionType != "channel.chat.message")
            return;

        var eventData =
            root.GetProperty("payload")
            .GetProperty("event");

        var badges = eventData.GetProperty("badges");
        var permission = GetUserPermission(badges);

        var message =
            eventData
            .GetProperty("message")
            .GetProperty("text")
            .GetString();

        Console.WriteLine($"CHAT: {message}");

        if (!IsAllowedUser(badges))
        {
            Console.WriteLine("Brak uprawnień do komendy");

            return;
        }

        if (string.IsNullOrWhiteSpace(message))
            return;

        if (message.Equals("!bingoreset", StringComparison.OrdinalIgnoreCase))
        {
            if (permission != TwitchUserPermission.Broadcaster)
            {
                Console.WriteLine("Brak uprawnień do resetu");
                return;
            }

            await HandleBingoResetAsync(cancellationToken);
            return;
        }

        if (message.StartsWith("!bingo ", StringComparison.OrdinalIgnoreCase))
        {
            if (permission == TwitchUserPermission.None)
            {
                Console.WriteLine("Widz próbował użyć bingo");
                return;
            }

            await HandleBingoCommandAsync(message, cancellationToken);
            return;
        }
    }

    private async Task CreateChatSubscriptionAsync(
    string sessionId,
    CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<BingoDbContext>();

        var auth = await db.TwitchAuth.FirstOrDefaultAsync(cancellationToken);

        if (auth == null)
        {
            Console.WriteLine("Brak autoryzacji Twitch");

            return;
        }

        var client = _httpClientFactory.CreateClient();

        client.DefaultRequestHeaders.Add("Authorization",$"Bearer {auth.AccessToken}");

        client.DefaultRequestHeaders.Add("Client-Id", _options.ClientId);

        var body = new
        {
            type = "channel.chat.message",

            version = "1",

            condition = new
            {
                broadcaster_user_id = auth.BroadcasterId,

                user_id = auth.BroadcasterId
            },

            transport = new
            {
                method = "websocket",

                session_id = sessionId
            }
        };

        var response = await client.PostAsJsonAsync("https://api.twitch.tv/helix/eventsub/subscriptions", body, cancellationToken);

        var content =await response.Content.ReadAsStringAsync(cancellationToken);

        Console.WriteLine("Subskrypcja Twitch:");

        Console.WriteLine(content);

        response.EnsureSuccessStatusCode();
    }

    private async Task HandleBingoCommandAsync(string message, CancellationToken cancellationToken)
    {
        var parts = message.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 2)
            return;

        if (!int.TryParse(parts[1], out var tileNumber))
            return;

        if (tileNumber < 1 || tileNumber > 25)
            return;

        Console.WriteLine($"Aktywuję kafelek {tileNumber}");

        using var scope = _scopeFactory.CreateScope();

        var bingoService = scope.ServiceProvider.GetRequiredService<BingoService>();

        await bingoService.ToggleTileByPositionAsync(tileNumber);
    }

    private async Task HandleBingoResetAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Reset bingo");
        using var scope = _scopeFactory.CreateScope();

        var bingoService = scope.ServiceProvider.GetRequiredService<BingoService>();

        await bingoService.ResetAsync();
    }

    private static bool IsAllowedUser(JsonElement badges)
    {
        foreach (var badge in badges.EnumerateArray())
        {
            var setId = badge.GetProperty("set_id").GetString();

            if (setId == "broadcaster" || setId == "moderator")
                return true;
        }

        return false;
    }

    private static TwitchUserPermission GetUserPermission(JsonElement badges)
    {
        foreach (var badge in badges.EnumerateArray())
        {
            var setId = badge
                .GetProperty("set_id")
                .GetString();

            if (setId == "broadcaster")
            {
                return TwitchUserPermission.Broadcaster;
            }

            if (setId == "moderator")
            {
                return TwitchUserPermission.Moderator;
            }
        }

        return TwitchUserPermission.None;
    }

    private async Task<bool> IsTwitchAuthorizedAsync()
    {
        using var scope = _scopeFactory.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<BingoDbContext>();

        return await db.TwitchAuth.AnyAsync();
    }

    private async Task RunEventSubAsync(
    CancellationToken stoppingToken)
    {
        using var socket = new ClientWebSocket();

        await socket.ConnectAsync(new Uri("wss://eventsub.wss.twitch.tv/ws"), stoppingToken);

        Console.WriteLine("Połączono z Twitch EventSub");

        var buffer = new byte[8192];

        while (!stoppingToken.IsCancellationRequested)
        {
            var result = await socket.ReceiveAsync(buffer,stoppingToken);
            var json = Encoding.UTF8.GetString(buffer, 0, result.Count);

            Console.WriteLine(json);

            using var document = JsonDocument.Parse(json);

            var messageType =
                document.RootElement
                .GetProperty("metadata")
                .GetProperty("message_type")
                .GetString();

            if (messageType == "session_welcome")
            {
                var sessionId =
                    document.RootElement
                    .GetProperty("payload")
                    .GetProperty("session")
                    .GetProperty("id")
                    .GetString();


                Console.WriteLine(
                    $"Session ID: {sessionId}");


                await CreateChatSubscriptionAsync(sessionId!, stoppingToken);
            }

            if (messageType == "notification")
            {
                await HandleNotificationAsync(document, stoppingToken);
            }
        }
    }
}