using BingoOverlay.Data;
using BingoOverlay.Hubs;
using BingoOverlay.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BingoOverlay.Pages.Admin;

public class IndexModel : PageModel
{
    private readonly BingoDbContext _db;
    private readonly IHubContext<BingoHub> _hub;


    public List<BingoTile> Tiles { get; set; } = [];


    public IndexModel(
        BingoDbContext db,
        IHubContext<BingoHub> hub)
    {
        _db = db;
        _hub = hub;
    }


    public async Task OnGetAsync()
    {
        Tiles = await _db.Tiles
            .OrderBy(x => x.Position)
            .ToListAsync();
    }


    public async Task<IActionResult> OnPostToggleAsync(int id)
    {
        var tile = await _db.Tiles
            .FirstOrDefaultAsync(x => x.Id == id);


        if (tile == null)
            return NotFound();


        tile.Completed = !tile.Completed;


        await _db.SaveChangesAsync();


        await _hub.Clients.All.SendAsync(
            "TileUpdated",
            tile.Id,
            tile.Completed);


        return new JsonResult(new
        {
            id = tile.Id,
            completed = tile.Completed
        });
    }
}