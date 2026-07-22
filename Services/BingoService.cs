using BingoOverlay.Data;
using BingoOverlay.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BingoOverlay.Services;

public class BingoService
{
    private readonly BingoDbContext _db;
    private readonly IHubContext<BingoHub> _hub;


    public BingoService(
        BingoDbContext db,
        IHubContext<BingoHub> hub)
    {
        _db = db;
        _hub = hub;
    }


    public async Task ToggleTileByPositionAsync(int position)
    {
        var tile = await _db
            .Tiles
            .FirstOrDefaultAsync(x => x.Position == position);

        if (tile == null)
            return;

        tile.Completed = !tile.Completed;

        await _db.SaveChangesAsync();

        await _hub.Clients.All.SendAsync(
            "TileUpdated",
            tile.Id,
            tile.Completed);
    }

    public async Task ResetAsync()
    {
        var tiles = await _db
            .Tiles
            .ToListAsync();

        foreach (var tile in tiles)
        {
            tile.Completed = false;
        }

        await _db.SaveChangesAsync();

        await _hub.Clients.All.SendAsync("BoardReset");
    }
}