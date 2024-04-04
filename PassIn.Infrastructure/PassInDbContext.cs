using Microsoft.EntityFrameworkCore;

using PassIn.Infrastructure.Entities;

namespace PassIn.Infrastructure;

public class PassInDbContext : DbContext
{
    private static string SqliteDatabasePath = string.Empty;

    public DbSet<Event> Events { get; set; }

    public DbSet<Attendee> Attendees { get; set; }

    public DbSet<CheckIn> CheckIns { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source = {SqliteDatabasePath}");
    }

    public static void InitializeDatabasePath(string path)
    {
        SqliteDatabasePath = path;
    }
}
