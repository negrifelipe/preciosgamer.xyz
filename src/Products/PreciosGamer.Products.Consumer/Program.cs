using MassTransit;
using Microsoft.EntityFrameworkCore;
using PreciosGamer.Products.Consumer.Handlers;
using PreciosGamer.Products.Persistence;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("/Logs/log.txt", fileSizeLimitBytes: null, rollingInterval: RollingInterval.Day, retainedFileCountLimit: null)
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();
    builder.Services.AddDbContext<ProductsDbContext>(x => x.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));

    builder.Services.AddMassTransit(x =>
    {
        x.AddConsumer<ProductScrappedHandler>();

        x.UsingRabbitMq((context, cfg) =>
        {
            cfg.Host(new Uri(builder.Configuration["RabbitMQ:Host"]!), c =>
            {
                c.Username(builder.Configuration["RabbitMQ:Username"]!);
                c.Password(builder.Configuration["RabbitMQ:Password"]!);
            });

            cfg.ConfigureEndpoints(context);
        });
    });

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