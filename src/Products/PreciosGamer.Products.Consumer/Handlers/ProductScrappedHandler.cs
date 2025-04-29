using MassTransit;
using Microsoft.EntityFrameworkCore;
using PreciosGamer.Products.Dtos.Events;
using PreciosGamer.Products.Persistence;

namespace PreciosGamer.Products.Consumer.Handlers;

public class ProductScrappedHandler : IConsumer<ProductScrapped>
{
    private readonly ILogger<ProductScrappedHandler> _logger;
    private readonly ProductsDbContext _context;

    public ProductScrappedHandler(ILogger<ProductScrappedHandler> logger, ProductsDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task Consume(ConsumeContext<ProductScrapped> context)
    {
        try
        {
            await ProcessScrappedProduct(context.Message);
        }
        catch(Exception)
        {
            _logger.LogError("Failed to process product {ProductSKU} from {ProductStoreId} {@ScrappedProduct}", context.Message.SKU, context.Message.StoreId, context.Message);
            throw;
        }
    }

    private async Task ProcessScrappedProduct(ProductScrapped scrappedProduct)
    {
        _logger.LogDebug("Processing product {ProductSKU} from {ProductStoreId} {ScrappedProduct}", scrappedProduct.SKU, scrappedProduct.StoreId, scrappedProduct);

        var product = await _context.Products.FirstOrDefaultAsync(x => x.SKU == scrappedProduct.SKU && x.StoreId == scrappedProduct.StoreId);

        if (product is not null)
        {
            product.Name = scrappedProduct.Details.Name;
            product.Price = scrappedProduct.Details.Price;
            product.Url = scrappedProduct.Details.Url;
            product.ImageUrl = scrappedProduct.Details.ImageUrl;

            _logger.LogDebug("Product {ProductSKU} from {ProductStoreId} already exists. Updating its information", scrappedProduct.SKU, scrappedProduct.StoreId);
            _context.Products.Update(product);
        }
        else
        {
            _logger.LogDebug("Product {ProductSKU} from {ProductStoreId} does not exist. Adding it to the database", scrappedProduct.SKU, scrappedProduct.StoreId);
            var entry = await _context.Products.AddAsync(new()
            {
                SKU = scrappedProduct.SKU,
                CreateDate = scrappedProduct.ScrappedDate,
                StoreId = scrappedProduct.StoreId,
                Name = scrappedProduct.Details.Name,
                Price = scrappedProduct.Details.Price,
                Url = scrappedProduct.Details.Url,
                ImageUrl = scrappedProduct.Details.ImageUrl,
            });

            product = entry.Entity;
        }

        var price = await _context.ProductPrices.FirstOrDefaultAsync(x => x.SKU == scrappedProduct.SKU && x.StoreId == scrappedProduct.StoreId && x.CreateDate == scrappedProduct.ScrappedDate);

        if (price is not null)
        {
            _logger.LogDebug("Product price {ProductSKU} from {ProductStoreId} has already been recorded. Updating it with the new data", scrappedProduct.SKU, scrappedProduct.StoreId);

            price.Price = scrappedProduct.Details.Price;
        }
        else
        {
            _logger.LogDebug("Product price {ProductSKU} from {ProductStoreId} will be added to the database", scrappedProduct.SKU, scrappedProduct.StoreId);

            var entry = await _context.ProductPrices.AddAsync(new()
            {
                SKU = scrappedProduct.SKU,
                StoreId = scrappedProduct.StoreId,
                Price = scrappedProduct.Details.Price,
                CreateDate = scrappedProduct.ScrappedDate
            });

            price = entry.Entity;
        }

        await _context.SaveChangesAsync();

        _logger.LogDebug("Finished processing product {ProductSKU} from {ProductStoreId}", scrappedProduct.SKU, scrappedProduct.StoreId);
    }
}
