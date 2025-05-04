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
        // dont add this to the query where. it will calculate the date in the db instead of here
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var date30DaysAgo = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30));

        var product = await context.Products
            .AsNoTracking()
            .Where(x => x.StoreId == storeId && x.SKU == productSKU)
            .Select(x => new ProductWithPricesResponse(
                x.SKU, 
                x.StoreId, 
                x.CreateDate, 
                new ProductDetails(x.Name, x.Url, x.ImageUrl, x.Price),
                context.ProductPrices
                    .Where(p => p.StoreId == x.StoreId && p.SKU == x.SKU && (p.CreateDate >= date30DaysAgo && p.CreateDate <= date))
                    .OrderByDescending(x => x.CreateDate)
                    .Select(x => new ProductPriceResponse(x.SKU, x.StoreId, x.CreateDate, x.Price))
                    .ToList()))
            .FirstOrDefaultAsync();

        if(product is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(product);
    });

    app.MapGet("/{StoreId}/{ProductSKU}/prices", async (
       ProductsDbContext context,
       [FromRoute(Name = "StoreId")] int storeId,
       [FromRoute(Name = "ProductSKU")] string productSKU,
       [FromQuery(Name = "startDate")] DateOnly? startDate,
       [FromQuery(Name = "endDate")] DateOnly? endDate) =>
    {
        var query = context.ProductPrices
            .AsNoTracking()
            .OrderByDescending(x => x.CreateDate)
            .Where(x => x.SKU == productSKU && x.StoreId == storeId);

        if(startDate is not null)
        {
            query = query.Where(x => x.CreateDate >= startDate);
        }

        if(endDate is not null)
        {
            query = query.Where(x => x.CreateDate <= endDate);
        }

        var prices = await query
            .Select(x => new ProductPriceResponse(x.SKU, x.StoreId, x.CreateDate, x.Price))
            .ToListAsync();

        return Results.Ok(prices);
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