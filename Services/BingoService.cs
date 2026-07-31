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

        var isBingo = await CheckBingoAsync();

        if (isBingo)
        {
            await _hub.Clients.All.SendAsync(
                "BingoAchieved"
            );
        }
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

    private async Task<bool> CheckBingoAsync()
    {
        var tiles = await _db.Tiles
            .OrderBy(x => x.Id)
            .ToListAsync();


        if (tiles.Count != 25)
            return false;

        var board = tiles
            .Select(x => x.Completed)
            .ToArray();

        for (int row = 0; row < 5; row++)
        {
            bool bingo = true;

            for (int col = 0; col < 5; col++)
            {
                if (!board[row * 5 + col])
                {
                    bingo = false;
                    break;
                }
            }

            if (bingo)
                return true;
        }

        for (int col = 0; col < 5; col++)
        {
            bool bingo = true;

            for (int row = 0; row < 5; row++)
            {
                if (!board[row * 5 + col])
                {
                    bingo = false;
                    break;
                }
            }

            if (bingo)
                return true;
        }

        bool diagonal1 = true;

        for (int i = 0; i < 5; i++)
        {
            if (!board[i * 5 + i])
            {
                diagonal1 = false;
                break;
            }
        }

        if (diagonal1)
            return true;

        bool diagonal2 = true;

        for (int i = 0; i < 5; i++)
        {
            if (!board[i * 5 + (4 - i)])
            {
                diagonal2 = false;
                break;
            }
        }

        if (diagonal2)
            return true;

        return false;
    }
}