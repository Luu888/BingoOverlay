using BingoOverlay.Data;
using BingoOverlay.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
namespace BingoOverlay.Hubs;


public class BingoHub : Hub
{
    private readonly BingoDbContext _db;
    private readonly CountdownService _countdownService;

    public BingoHub(BingoDbContext db, CountdownService countdownService)
    {
        _db = db;
        _countdownService = countdownService;
    }

    public override async Task OnConnectedAsync()
    {
        var settings = await _db.Settings
            .FirstOrDefaultAsync();

        await Clients.Caller.SendAsync(
            "OverlayVisibilityChanged",
            new
            {
                type = "overlayVisibility",
                visible = settings?.IsOverlayVisible ?? true
            });

        await base.OnConnectedAsync();
    }

    public async Task UpdateTile(int id, bool completed)
    {
        await Clients.All.SendAsync(
            "TileUpdated",
            id,
            completed);
    }

    public async Task UpdateAppearance(object appearance)
    {
        await Clients.All.SendAsync(
            "AppearanceUpdated",
            appearance);
    }

    public async Task SetOverlayVisibility(bool visible)
    {
        await Clients.All.SendAsync(
            "OverlayVisibilityChanged",
            new
            {
                type = "overlayVisibility",
                visible
            });
    }

    public async Task SetCountdownDuration(int minutes)
    {
        await _countdownService.SetDurationAsync(minutes);
    }

    public async Task StartCountdown()
    {
        await _countdownService.StartAsync();
    }

    public async Task PauseCountdown()
    {
        await _countdownService.PauseAsync();
    }

    public async Task ResetCountdown()
    {
        await _countdownService.ResetAsync();
    }
}