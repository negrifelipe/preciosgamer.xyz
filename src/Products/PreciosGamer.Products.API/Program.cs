using Serilog.Events;
using Serilog;
using Microsoft.EntityFrameworkCore;
using PreciosGamer.Products.Persistence;
using Microsoft.AspNetCore.Mvc;
using PreciosGamer.Products.Dtos;
using System.Text.Json;

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
    
    builder.Services.AddDbContext<ProductsDbContext>(x => x.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));

    var app = builder.Build();

    app.UseHttpsRedirection();

    app.MapGet("/", async (
        ProductsDbContext context, 
        [FromQuery(Name = "p")] int page = 0, 
        [FromQuery(Name = "s")] int pageSize = 50,
        [FromQuery(Name = "q")] string? search = null) =>
    {
        var productsQuery = context.Products
            .AsNoTracking()
            .OrderByDescending(x => x.CreateDate);

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
            .Select(x => new ProductResponse(x.SKU, x.StoreId, x.CreateDate, new ProductDetails(x.Name, x.Url, x.ImageUrl, x.Price)))
            .ToListAsync();

        var productsCount = await productsQuery.CountAsync();

        return Results.Ok(new PaginatedResponse<ProductResponse>(products, page, pageSize, productsCount));
    });

    app.MapGet("/{StoreId}/{ProductSKU}", async (
        ProductsDbContext context,
        [FromRoute(Name = "StoreId")] int storeId,
        [FromRoute(Name = "ProductSKU")] string productSKU) =>
    {
        var product = await context.Products
            .AsNoTracking()
            .Where(x => x.StoreId == storeId && x.SKU == productSKU)
            .Select(x => new ProductWithPricesResponse(
                x.SKU, 
                x.StoreId, 
                x.CreateDate, 
                new ProductDetails(x.Name, x.Url, x.ImageUrl, x.Price),
                context.ProductPrices
                    .Where(p => p.StoreId == x.StoreId && p.SKU == x.SKU)
                    .OrderByDescending(x => x.CreateDate)
                    .Take(30)
                    .Select(x => new ProductPriceResponse(x.SKU, x.StoreId, x.CreateDate, x.Price))
                    .ToList()))
            .FirstOrDefaultAsync();

        if(product is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(product);
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