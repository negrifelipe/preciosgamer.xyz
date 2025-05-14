using Microsoft.EntityFrameworkCore;
using PreciosGamer.ExchangeRates.API.Persistence.Entities;
using System.Reflection.Emit;

namespace PreciosGamer.ExchangeRates.API.Persistence;

public class ExchangeRatesDbContext : DbContext
{
    public DbSet<ExchangeRateEntity> ExchangeRates { get; set; }

    public ExchangeRatesDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}
