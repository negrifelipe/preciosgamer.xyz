using Serilog.Events;
using Serilog;
using Microsoft.EntityFrameworkCore;
using PreciosGamer.ExchangeRates.API.Persistence;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("/Logs/log.txt", fileSizeLimitBytes: null, rollingInterval: RollingInterval.Day, retainedFileCountLimit: null)
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddSerilog((services, lc) => lc
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services));

    builder.Services.AddDbContext<ExchangeRatesDbContext>(x => x.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));

    var app = builder.Build();

    app.UseHttpsRedirection();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}