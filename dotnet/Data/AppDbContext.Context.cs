using Microsoft.EntityFrameworkCore;
using CloudApplication.Models;

namespace CloudApplication.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Item> Items => Set<Item>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("orders", schema: "public");
            entity.HasKey(e => e.id);
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.ToTable("items", schema: "public");
            entity.HasKey(e => e.id);
            entity.HasOne<Order>()
                  .WithMany(o => o.Items)
                  .HasForeignKey(i => i.order_id)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}