using BingoOverlay.Data;
using BingoOverlay.Hubs;
using BingoOverlay.Models;
using BingoOverlay.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Diagnostics;


var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:8888");

builder.Services.AddRazorPages();

builder
    .Services
    .AddControllersWithViews()
    .AddRazorOptions(options =>
    {
        options.ViewLocationFormats.Add("/Pages/Shared/{0}.cshtml");
    });

builder.Services.AddHttpClient();

builder.Services.AddDbContext<BingoDbContext>(
    options =>
    {
        options.UseSqlite(
            "Data Source=bingo.db");
    });


builder.Services.AddSignalR();

builder.Services.AddScoped<BingoService>();

builder.Services.AddHostedService<TwitchEventSubService>();

builder.Services.Configure<TwitchOptions>(
    builder.Configuration.GetSection("Twitch"));

var app = builder.Build();


app.UseStaticFiles();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<BingoHub>("/bingoHub");




using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
        .GetRequiredService<BingoDbContext>();

    await db.Database.MigrateAsync();

    if (!await db.Tiles.AnyAsync())
    {
        for (int i = 0; i < 25; i++)
        {
            db.Tiles.Add(
                new BingoTile
                {
                    Position = i + 1,
                    Text = $"Pole {i + 1}"
                });
        }

        await db.SaveChangesAsync();
    }

    if (!await db.BingoAppearance.AnyAsync())
    {
        db.BingoAppearance.Add(
            new BingoAppearance
            {
                TileColor = "#ff8fd8",
                TileColorEnd = "#c99cff",
                CompletedColor = "#7dffb2",
                CompletedColorEnd = "#32d583",
                TextColor = "#ffffff",
                BorderRadius = 22,
                TileSize = 120
            });

        await db.SaveChangesAsync();
    }

    if (!await db.Settings.AnyAsync())
    {
        db.Settings.Add(
            new Settings
            {
                AllowModerators = true,
                AllowVips = false,
                HideOverlayAfterTime = false,
                HideOverlaySeconds = 60,
                IsOverlayVisible = true,
                LastOverlayActivity = DateTime.UtcNow
            });

        await db.SaveChangesAsync();
    }
}

app.Lifetime.ApplicationStarted.Register(() =>
{
    Process.Start(new ProcessStartInfo
    {
        FileName = "http://localhost:8888",
        UseShellExecute = true
    });
});

app.Run();