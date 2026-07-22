using BingoOverlay.Data;
using BingoOverlay.Hubs;
using BingoOverlay.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Diagnostics;


var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:8888");

builder.Services.AddRazorPages();


builder.Services.AddDbContext<BingoDbContext>(
    options =>
    {
        options.UseSqlite(
            "Data Source=bingo.db");
    });


builder.Services.AddSignalR();


var app = builder.Build();


app.UseStaticFiles();

app.MapRazorPages();

app.MapHub<BingoHub>("/bingoHub");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
                  .GetRequiredService<BingoDbContext>();

    db.Database.EnsureCreated();

    if (!db.Tiles.Any())
    {
        for (int i = 0; i < 25; i++)
        {
            db.Tiles.Add(
                new BingoTile
                {
                    Position = i,
                    Text = $"Pole {i + 1}"
                });
        }

        db.SaveChanges();
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