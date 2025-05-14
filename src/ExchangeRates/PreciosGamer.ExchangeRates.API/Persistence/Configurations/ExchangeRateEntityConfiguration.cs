using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PreciosGamer.ExchangeRates.API.Persistence.Entities;

namespace PreciosGamer.ExchangeRates.API.Persistence.Configurations;

public class ExchangeRateEntityConfiguration : IEntityTypeConfiguration<ExchangeRateEntity>
{
    public void Configure(EntityTypeBuilder<ExchangeRateEntity> builder)
    {
        builder.ToTable("ExchangeRates");
        builder.HasKey(x => new { x.BaseCurrencyCode, x.TargetCurrencyCode, x.Date });

        builder.Property(x => x.BaseCurrencyCode)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(x => x.TargetCurrencyCode)
            .HasMaxLength(3)
            .IsRequired();

        // 2 decimal numbers is enough for my use case
        builder.Property(x => x.ConversionRate)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(x => x.Date).IsRequired();
    }
}
