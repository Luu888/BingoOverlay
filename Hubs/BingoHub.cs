using BingoOverlay.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
namespace BingoOverlay.Hubs;


public class BingoHub : Hub
{
    private readonly BingoDbContext _db;

    public BingoHub(BingoDbContext db)
    {
        _db = db;
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
}