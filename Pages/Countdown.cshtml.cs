using BingoOverlay.Models;
using BingoOverlay.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BingoOverlay.Pages;

public class CountdownModel : PageModel
{
    private readonly CountdownService _countdownService;

    public CountdownSettings Settings { get; private set; } = null!;

    public CountdownModel(CountdownService countdownService)
    {
        _countdownService = countdownService;
    }

    public async Task OnGetAsync()
    {
        Settings = await _countdownService.GetAsync();
    }
}