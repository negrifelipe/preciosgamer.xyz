using Serilog.Events;
using Serilog;
using Microsoft.EntityFrameworkCore;
using PreciosGamer.Products.Persistence;
using Microsoft.AspNetCore.Mvc;
using PreciosGamer.Products.Persistence.Entities;

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

    var app = builder.Build();

    app.UseHttpsRedirection();

    app.MapGet("/", async (ProductsDbContext context, [FromQuery(Name = "p")] int page = 0, [FromQuery(Name = "s")] int pageSize = 50, [FromQuery(Name = "q")] string? search = null) =>
    {
        var productsQuery = context.Products
            .AsNoTracking();

        if (page < 0)
        {
            page = 0;
        }

        if(pageSize > 100)
        {
            pageSize = 100;
        }

        if (search is not null)
        {
            productsQuery = productsQuery
                .Where(x => x.SearchVector.Matches(EF.Functions.PlainToTsQuery("spanish", search)))
                .OrderByDescending(x => x.SearchVector.Rank(EF.Functions.PlainToTsQuery("spanish", search)));
        }

        var products = await productsQuery
            .Skip(page * pageSize)
            .Take(pageSize)
            .Select(x => new
            {
                x.SKU,
                x.StoreId,
                x.Name,
                x.Price,
                x.Url,
                x.ImageUrl,
                x.CreateDate
            })
            .ToListAsync();

        var productsCount = await productsQuery.CountAsync();

        return Results.Ok(new
        {
            data = products,
            page,
            pageSize,
            hasNextPage = productsCount > (page * pageSize) + products.Count,
            hasPreviousPage = page > 0
        });
    });

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