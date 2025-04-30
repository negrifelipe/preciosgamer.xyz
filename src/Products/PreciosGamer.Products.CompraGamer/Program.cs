using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PreciosGamer.Products.CompraGamer.Jobs;
using PreciosGamer.Products.CompraGamer.Services;
using Quartz;
using Serilog.Events;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("/Logs/log.txt", fileSizeLimitBytes: null, rollingInterval: RollingInterval.Day, retainedFileCountLimit: null)
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .CreateLogger();

try
{
    var builder = Host.CreateApplicationBuilder(args);
    builder.Services.AddSerilog();
    builder.Services.AddSingleton<CompraGamerProductFetcherService>();
    builder.Services.AddQuartz(configure =>
    {
        var compraGamerProductsJobKey = new JobKey(nameof(CompraGamerProductsJob));

        configure
            .AddJob<CompraGamerProductsJob>(compraGamerProductsJobKey)
            .AddTrigger(trigger =>
            {
                trigger
                    .ForJob(compraGamerProductsJobKey)
                    .WithCronSchedule(builder.Configuration["ProductsJobCronSchedule"] ?? "0 0 0 ? * *");
            });
    });

    builder.Services.AddQuartzHostedService();

    builder.Services.AddMassTransit(x =>
    {
        x.UsingRabbitMq((context, cfg) =>
        {
            cfg.Host(new Uri(builder.Configuration["RabbitMQ:Host"]!), c =>
            {
                c.Username(builder.Configuration["RabbitMQ:Username"]!);
                c.Password(builder.Configuration["RabbitMQ:Password"]!);
            });
        });
    });

    var app = builder.Build();

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