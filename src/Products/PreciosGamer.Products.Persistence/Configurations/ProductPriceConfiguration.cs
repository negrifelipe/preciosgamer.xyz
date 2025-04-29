using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PreciosGamer.Products.Persistence.Entities;

namespace PreciosGamer.Products.Persistence.Configurations;

public class ProductPriceConfiguration : IEntityTypeConfiguration<ProductPriceEntity>
{
    public void Configure(EntityTypeBuilder<ProductPriceEntity> builder)
    {
        builder.ToTable("ProductPrices");
        builder.HasKey(x => new { x.SKU, x.StoreId, x.CreateDate });

        builder.Property(x => x.SKU).HasMaxLength(64);

        builder.Property(x => x.Price)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);
    }
}
