using CommandService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandService.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }

    public DbSet<Platform> Platforms { get; set; }
    public DbSet<Command> Commands { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Platform>()
            .HasMany(p => p.Commands)
            .WithOne(c => c.Platform)
            .HasForeignKey(c => c.PlatformId);

        modelBuilder
            .Entity<Command>()
            .HasOne(c => c.Platform)
            .WithMany(p => p.Commands)
            .HasForeignKey(c => c.PlatformId);
    }
}