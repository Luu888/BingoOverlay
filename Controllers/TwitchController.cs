using System.Text.Json;
using BingoOverlay.Data;
using BingoOverlay.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BingoOverlay.Controllers;

[Route("twitch")]
public class TwitchController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly BingoDbContext _db;
    private readonly IHttpClientFactory _httpClientFactory;


    public TwitchController(
        IConfiguration configuration,
        BingoDbContext db,
        IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _db = db;
        _httpClientFactory = httpClientFactory;
    }



    [HttpGet("connect")]
    public IActionResult Connect()
    {
        var clientId = _configuration["Twitch:ClientId"];

        var redirectUri = _configuration["Twitch:RedirectUri"];

        var url =
            "https://id.twitch.tv/oauth2/authorize" +
            "?response_type=code" +
            $"&client_id={clientId}" +
            $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
            "&scope=user:read:chat+user:write:chat";


        return Redirect(url);
    }

    [HttpGet("callback")]
    public async Task<IActionResult> Callback(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return BadRequest("Brak code");

        var client = _httpClientFactory.CreateClient();
        var clientId = _configuration["Twitch:ClientId"];
        var clientSecret = _configuration["Twitch:ClientSecret"];
        var redirectUri = _configuration["Twitch:RedirectUri"];

        if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret) || string.IsNullOrWhiteSpace(redirectUri))
        {
            return BadRequest("Brak konfiguracji Twitch w appsettings.json");
        }

        var tokenRequest =
            new Dictionary<string, string>
            {
                ["client_id"] = clientId!,
                ["client_secret"] = clientSecret!,
                ["code"] = code,
                ["grant_type"] = "authorization_code",
                ["redirect_uri"] = redirectUri
            };

        var tokenResponse = await client.PostAsync(
                "https://id.twitch.tv/oauth2/token",
                new FormUrlEncodedContent(tokenRequest));

        var tokenJson = await tokenResponse.Content.ReadAsStringAsync();

        if (!tokenResponse.IsSuccessStatusCode)
        {
            return BadRequest(tokenJson);
        }

        var token = JsonSerializer.Deserialize<TwitchTokenResponse>(tokenJson);

        if (token == null)
            return BadRequest("Nie udało się pobrać tokena");

        client.DefaultRequestHeaders.Clear();

        client.DefaultRequestHeaders.Add(
            "Authorization",
            $"Bearer {token.AccessToken}");

        client.DefaultRequestHeaders.Add("Client-Id", clientId);

        var userResponse = await client.GetAsync("https://api.twitch.tv/helix/users");
        var userJson = await userResponse.Content.ReadAsStringAsync();
        var user = JsonSerializer.Deserialize<TwitchUserResponse>(userJson);

        var twitchUser = user?.Data.FirstOrDefault();

        if (twitchUser == null)
            return BadRequest("Nie znaleziono użytkownika Twitch");

        var old = await _db.TwitchAuth.ToListAsync();

        _db.TwitchAuth.RemoveRange(old);

        _db.TwitchAuth.Add(
            new TwitchAuth
            {
                AccessToken = token.AccessToken,
                RefreshToken = token.RefreshToken,
                BroadcasterId = twitchUser.Id,
                DisplayName = twitchUser.DisplayName,
                ExpiresAt = DateTime.UtcNow.AddSeconds(token.ExpiresIn)
            });

        await _db.SaveChangesAsync();

        return Redirect("/?twitch=connected");
    }

    [HttpGet("status")]
    public async Task<IActionResult> Status()
    {
        var auth = await _db.TwitchAuth.FirstOrDefaultAsync();

        return View(auth);
    }
}