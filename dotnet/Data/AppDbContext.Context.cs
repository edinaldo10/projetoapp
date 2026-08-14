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
            entity.ToTable("orders");
            entity.HasKey(e => e.id);
            entity.Property(e => e.id).HasColumnName("id");
            entity.Property(e => e.customer).HasColumnName("customer");
            entity.Property(e => e.status).HasColumnName("status");
            entity.Property(e => e.created_at).HasColumnName("created_at");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.ToTable("items");
            entity.HasKey(e => e.id);
            entity.Property(e => e.id).HasColumnName("id");
            entity.Property(e => e.order_id).HasColumnName("order_id");
            entity.Property(e => e.sku).HasColumnName("sku");
            entity.Property(e => e.description).HasColumnName("description");
            entity.Property(e => e.quantity).HasColumnName("quantity");

            // Mapeamento correto utilizando a navegação da entidade Order
            entity.HasOne(i => i.order)
                  .WithMany(o => o.Items)
                  .HasForeignKey(i => i.order_id)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}