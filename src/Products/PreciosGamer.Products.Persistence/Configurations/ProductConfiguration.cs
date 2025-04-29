using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PreciosGamer.Products.Persistence.Entities;

namespace PreciosGamer.Products.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<ProductEntity>
{
    public void Configure(EntityTypeBuilder<ProductEntity> builder)
    {
        builder.ToTable("Products");
        builder.HasKey(x => new { x.SKU, x.StoreId });

        builder.Property(x => x.SKU).HasMaxLength(64);

        builder.Property(x => x.Name).HasMaxLength(300);
        
        builder.Property(x => x.Url).HasMaxLength(600);

        builder.Property(x => x.ImageUrl)
            .HasMaxLength(600)
            .IsRequired(false);

        builder.Property(x => x.Price)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.HasGeneratedTsVectorColumn(x => x.SearchVector, "spanish", x => new { x.SKU, x.Name })
            .HasIndex(x => x.SearchVector)
            .HasMethod("GIN");

        builder.HasMany<ProductPriceEntity>().WithOne().HasForeignKey(nameof(ProductPriceEntity.SKU), nameof(ProductPriceEntity.StoreId));
    }
}
