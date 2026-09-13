using BingoOverlay.Data;
using BingoOverlay.Hubs;
using BingoOverlay.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BingoOverlay.Services;

public class CountdownService
{
    private readonly BingoDbContext _db;
    private readonly IHubContext<BingoHub> _hub;

    public CountdownService(
        BingoDbContext db,
        IHubContext<BingoHub> hub)
    {
        _db = db;
        _hub = hub;
    }

    public async Task<CountdownSettings> GetAsync()
    {
        var settings = await _db.CountdownSettings
            .FirstOrDefaultAsync();

        if (settings != null)
            return settings;

        settings = new CountdownSettings();

        _db.CountdownSettings.Add(settings);

        await _db.SaveChangesAsync();

        return settings;
    }

    public async Task SetDurationAsync(int minutes)
    {
        if (minutes <= 0)
            throw new ArgumentOutOfRangeException(nameof(minutes));

        var settings = await GetAsync();

        settings.DurationSeconds = minutes * 60;
        settings.RemainingSeconds = settings.DurationSeconds;
        settings.IsRunning = false;
        settings.EndsAt = null;

        await _db.SaveChangesAsync();

        await BroadcastAsync(settings);
    }

    public async Task StartAsync()
    {
        var settings = await GetAsync();

        if (settings.RemainingSeconds <= 0)
        {
            settings.RemainingSeconds = settings.DurationSeconds;
        }

        settings.EndsAt = DateTimeOffset.UtcNow
            .AddSeconds(settings.RemainingSeconds);

        settings.IsRunning = true;

        await _db.SaveChangesAsync();

        await BroadcastAsync(settings);
    }

    public async Task PauseAsync()
    {
        var settings = await GetAsync();

        if (settings.IsRunning && settings.EndsAt.HasValue)
        {
            settings.RemainingSeconds = GetRemainingSeconds(settings);
        }

        settings.IsRunning = false;
        settings.EndsAt = null;

        await _db.SaveChangesAsync();

        await BroadcastAsync(settings);
    }

    public async Task ResetAsync()
    {
        var settings = await GetAsync();

        settings.RemainingSeconds = settings.DurationSeconds;
        settings.IsRunning = false;
        settings.EndsAt = null;

        await _db.SaveChangesAsync();

        await BroadcastAsync(settings);
    }

    public async Task UpdateAppearanceAsync(
        string digitColor,
        string backgroundColor,
        bool transparentBackground)
    {
        var settings = await GetAsync();

        settings.DigitColor = digitColor;
        settings.BackgroundColor = backgroundColor;
        settings.TransparentBackground = transparentBackground;

        await _db.SaveChangesAsync();

        await BroadcastAsync(settings);
    }

    public static int GetRemainingSeconds(CountdownSettings settings)
    {
        if (!settings.IsRunning || !settings.EndsAt.HasValue)
        {
            return Math.Max(0, settings.RemainingSeconds);
        }

        var remaining = settings.EndsAt.Value - DateTimeOffset.UtcNow;

        return Math.Max(0, (int)Math.Ceiling(remaining.TotalSeconds));
    }

    private async Task BroadcastAsync(CountdownSettings settings)
    {
        await _hub.Clients.All.SendAsync(
            "CountdownUpdated",
            new
            {
                durationSeconds = settings.DurationSeconds,
                remainingSeconds = GetRemainingSeconds(settings),
                isRunning = settings.IsRunning,
                endsAt = settings.EndsAt,
                digitColor = settings.DigitColor,
                backgroundColor = settings.BackgroundColor,
                transparentBackground = settings.TransparentBackground
            });
    }
}