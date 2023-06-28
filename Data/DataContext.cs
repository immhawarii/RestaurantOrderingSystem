using Microsoft.EntityFrameworkCore;
using RestaurantOrderingSystem.Models;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    public DbSet<MenuItem> MenuItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Log> Logs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>()
            .Property(e => e.MenuItemIds)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList()
            );

        modelBuilder.Entity<MenuItem>()
            .HasKey(e => e.ID);

        modelBuilder.Entity<Log>()
            .HasKey(e => e.ID);

        base.OnModelCreating(modelBuilder);
    }
}
