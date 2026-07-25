using Microsoft.AspNetCore.SignalR;

namespace BingoOverlay.Hubs;


public class BingoHub : Hub
{
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