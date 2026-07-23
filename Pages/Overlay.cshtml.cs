using BingoOverlay.Data;
using BingoOverlay.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BingoOverlay.Pages;

public class OverlayModel : PageModel
{
    private readonly BingoDbContext _db;

    public List<BingoTile> Tiles { get; set; } = [];

    public BingoAppearance Appearance { get; set; } = null!;

    public OverlayModel(BingoDbContext db)
    {
        _db = db;
    }

    public async Task OnGet()
    {
        Tiles = await _db.Tiles
            .OrderBy(x => x.Position)
            .ToListAsync();

        Appearance = await _db.BingoAppearance.FirstAsync() ?? new();
    }
}