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

        // Força explicitamente o nome exato da tabela e o schema no PostgreSQL
        modelBuilder.Entity<Order>().ToTable("orders", schema: "public");
        modelBuilder.Entity<Item>().ToTable("items", schema: "public");
    }
}