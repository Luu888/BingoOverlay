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

app.MapControllers();

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
                    Position = i + 1,
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