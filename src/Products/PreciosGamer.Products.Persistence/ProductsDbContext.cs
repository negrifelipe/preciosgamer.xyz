using Microsoft.EntityFrameworkCore;
using PreciosGamer.Products.Persistence.Entities;

namespace PreciosGamer.Products.Persistence;

public class ProductsDbContext : DbContext
{
    public DbSet<ProductEntity> Products { get; set; }
    public DbSet<ProductPriceEntity> ProductPrices { get; set; }

    public ProductsDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}
