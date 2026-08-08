using BingoOverlay.Data;
using BingoOverlay.Hubs;
using BingoOverlay.Models;
using BingoOverlay.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BingoOverlay.Pages.Admin;

public class IndexModel : PageModel
{
    private readonly BingoDbContext _db;
    private readonly IHubContext<BingoHub> _hub;
    private readonly BingoService _bingoService;

    public List<BingoTile> Tiles { get; set; } = [];

    [BindProperty]
    public Settings Settings { get; set; } = default!;

    public IndexModel(BingoDbContext db, 
        IHubContext<BingoHub> hub,
        BingoService bingoService)
    {
        _db = db;
        _hub = hub;
        _bingoService = bingoService;
    }

    public async Task OnGetAsync()
    {
        Tiles = await _db.Tiles
            .OrderBy(x => x.Position)
            .ToListAsync();

        Settings = await _db.Settings.FirstOrDefaultAsync();

        if (Settings == null)
        {
            Settings = new Settings();
            _db.Settings.Add(Settings);
            await _db.SaveChangesAsync();
        }
    }

    public async Task<IActionResult> OnPostToggleAsync(int id)
    {
        await _bingoService.ToggleTileAsync(id);

        return new JsonResult(new
        {
            success = true
        });
    }


    public async Task<IActionResult> OnPostUpdateTextAsync(int id, string text)
    {
        var tile = await _db.Tiles.FirstOrDefaultAsync(x => x.Id == id);

        if (tile == null)
            return NotFound();

        tile.Text = text;

        await _db.SaveChangesAsync();

        await _hub.Clients.All.SendAsync(
            "TileTextUpdated",
            tile.Id,
            tile.Text);

        return new JsonResult(new
        {
            success = true
        });
    }

    public async Task<IActionResult> OnPostSaveSettingsAsync()
    {
        var settings = await _db.Settings.FirstAsync();

        settings.AllowModerators = Settings.AllowModerators;
        settings.HideOverlayAfterTime = Settings.HideOverlayAfterTime;
        settings.HideOverlaySeconds = Math.Max(Settings.HideOverlaySeconds, 5);


        if (!settings.HideOverlayAfterTime)
        {
            settings.IsOverlayVisible = true;
        }
        else
        {
            settings.IsOverlayVisible = true;
            settings.LastOverlayActivity = DateTime.UtcNow;
        }


        await _db.SaveChangesAsync();


        await _hub.Clients.All.SendAsync(
            "OverlayVisibilityChanged",
            new
            {
                type = "overlayVisibility",
                visible = settings.IsOverlayVisible
            });


        return RedirectToPage();
    }
}