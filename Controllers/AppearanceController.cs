using BingoOverlay.Data;
using BingoOverlay.Hubs;
using BingoOverlay.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BingoOverlay.Controllers;

public class AppearanceController : Controller
{
    private readonly BingoDbContext _db;
    private readonly IHubContext<BingoHub> _hub;

    public AppearanceController(
        BingoDbContext db,
        IHubContext<BingoHub> hub)
    {
        _db = db;
        _hub = hub;
    }


    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var config = await _db.BingoAppearance.FirstAsync();

        return View(config);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(BingoAppearance model)
    {
        var config = await _db.BingoAppearance.FirstAsync();

        config.TileColor = model.TileColor;
        config.TileColorEnd = model.TileColorEnd;
        config.CompletedColor = model.CompletedColor;
        config.CompletedColorEnd = model.CompletedColorEnd;
        config.TextColor = model.TextColor;
        config.BorderRadius = model.BorderRadius;
        config.TileSize = model.TileSize;

        await _db.SaveChangesAsync();

        await _hub.Clients.All.SendAsync("AppearanceUpdated", config);

        return RedirectToAction(nameof(Index));
    }
}