using BingoOverlay.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using BingoOverlay.Models;

namespace BingoOverlay.ViewComponents;

public class TwitchStatusViewComponent : ViewComponent
{
    private readonly BingoDbContext _db;

    public TwitchStatusViewComponent(
        BingoDbContext db)
    {
        _db = db;
    }


    public async Task<IViewComponentResult> InvokeAsync()
    {
        var auth = await _db.TwitchAuth
            .FirstOrDefaultAsync();


        if (auth == null)
        {
            return View(new TwitchStatusViewModel
            {
                Connected = false
            });
        }


        return View(new TwitchStatusViewModel
        {
            Connected = true,
            DisplayName = auth.DisplayName
        });
    }
}