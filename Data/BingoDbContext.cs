using Microsoft.EntityFrameworkCore;
using BingoOverlay.Models;

namespace BingoOverlay.Data;

public class BingoDbContext : DbContext
{
    public BingoDbContext(
        DbContextOptions<BingoDbContext> options)
        : base(options)
    {
    }

    public DbSet<BingoTile> Tiles => Set<BingoTile>();
    public DbSet<TwitchAuth> TwitchAuth { get; set; }
}