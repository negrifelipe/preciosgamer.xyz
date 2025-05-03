using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PreciosGamer.Products.CompraGamer.Jobs;
using PreciosGamer.Products.CompraGamer.Services;
using Quartz;
using Serilog.Events;
using Serilog;
using Microsoft.Extensions.Logging;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("/Logs/log.txt", fileSizeLimitBytes: null, rollingInterval: RollingInterval.Day, retainedFileCountLimit: null)
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .CreateBootstrapLogger();

try
{
    var builder = Host.CreateApplicationBuilder(args);
    builder.Services.AddSerilog((services, lc) => lc
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services));
    builder.Services.AddSingleton<CompraGamerProductFetcherService>();
    builder.Services.AddQuartz(configure =>
    {
        configure
            .AddJob<CompraGamerProductsJob>(CompraGamerProductsJob.JobKey)
            .AddTrigger(trigger =>
            {
                trigger
                    .ForJob(CompraGamerProductsJob.JobKey)
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

    if (builder.Environment.IsDevelopment())
    {
        var logger = app.Services.GetRequiredService<Microsoft.Extensions.Logging.ILogger<Program>>();
        var schedulerFactory = app.Services.GetRequiredService<ISchedulerFactory>();
        var scheduler = await schedulerFactory.GetScheduler();
        logger.LogInformation("Development environment. Triggering the job automatically..");
        await scheduler.TriggerJob(CompraGamerProductsJob.JobKey);
        await scheduler.Standby();
        logger.LogInformation("Triggered all jobs");
    }

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