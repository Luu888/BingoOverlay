using BingoOverlay.Data;
using BingoOverlay.Hubs;
using BingoOverlay.Models;
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
        var tile = await _db.Tiles
        .FirstOrDefaultAsync(x => x.Position == position);

        if (tile == null)
            return;

        await ToggleTileAsync(tile);
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

    public async Task ShuffleAsync()
    {
        var tiles = await _db.Tiles
            .OrderBy(x => x.Position)
            .ToListAsync();

        var shuffledTiles = tiles
            .OrderBy(_ => Random.Shared.Next())
            .ToList();

        for (var i = 0; i < shuffledTiles.Count; i++)
        {
            shuffledTiles[i].Position = i + 1;
            shuffledTiles[i].Completed = false;
        }

        await _db.SaveChangesAsync();

        await _hub.Clients.All.SendAsync(
            "BoardShuffled",
            shuffledTiles
                .OrderBy(x => x.Position)
                .Select(x => new
                {
                    x.Id,
                    x.Position,
                    x.Text,
                    x.Completed
                }));
    }

    public async Task ToggleTileAsync(int id)
    {
        var tile = await _db.Tiles
            .FirstOrDefaultAsync(x => x.Id == id);

        if (tile == null)
            return;

        await ToggleTileAsync(tile);
    }

    private async Task ToggleTileAsync(BingoTile tile)
    {
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
                "BingoAchieved");
        }
    }

    private async Task<bool> CheckBingoAsync()
    {
        var tiles = await _db.Tiles
            .OrderBy(x => x.Position)
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

        return diagonal2;
    }
}