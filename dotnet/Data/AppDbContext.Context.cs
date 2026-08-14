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

        // Mapeamento fluído explícito para Order
        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("orders", schema: "public");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Customer).HasColumnName("customer");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
        });

        // Mapeamento fluído explícito para Item
        modelBuilder.Entity<Item>(entity =>
        {
            entity.ToTable("items", schema: "public");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Sku).HasColumnName("sku");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
        });
    }
}