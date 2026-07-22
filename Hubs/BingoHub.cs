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
}