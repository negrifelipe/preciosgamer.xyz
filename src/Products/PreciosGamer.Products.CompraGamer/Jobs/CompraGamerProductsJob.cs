using MassTransit;
using Microsoft.Extensions.Logging;
using PreciosGamer.Products.CompraGamer.Models;
using PreciosGamer.Products.CompraGamer.Services;
using PreciosGamer.Products.Dtos;
using PreciosGamer.Products.Dtos.Events;
using Quartz;

namespace PreciosGamer.Products.CompraGamer.Jobs;

public class CompraGamerProductsJob : IJob
{
    public static readonly JobKey JobKey = new(nameof(CompraGamerProductsJob));

    private readonly ILogger<CompraGamerProductsJob> _logger;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly CompraGamerProductFetcherService _productFetcherService;

    public CompraGamerProductsJob(ILogger<CompraGamerProductsJob> logger, IPublishEndpoint publishEndpoint, CompraGamerProductFetcherService productFetcherService)
    {
        _logger = logger;
        _publishEndpoint = publishEndpoint;
        _productFetcherService = productFetcherService;
    }

    private readonly int[] _ignoredCategories = [3, 0];

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Notifying products service with up to date product data");

        foreach(var product in await _productFetcherService.FetchProducts())
        {
            try
            {
                await ProcessProduct(product);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to process product {@Product}", product);
            }
        }

        _logger.LogInformation("Finished notifying job");
    }

    private async Task ProcessProduct(CompraGamerProduct product)
    {
        if (_ignoredCategories.Contains(product.IdCategoria))
        {
            _logger.LogDebug("Ignoring product {ProductId} from category {ProductCategoryId}", product.IdProducto, product.IdCategoria);
            return;
        }

        if (product.CodigoPrincipal?.Length == 0 || !(product.CodigoPrincipal?[0].Contains("SKU") ?? false) && !(product.CodigoPrincipal?[0].Contains("EAN") ?? false))
        {
            _logger.LogDebug("Ignoring product {ProductId} as it has no SKU", product.IdProducto);
            return;
        }

        string sku = product.CodigoPrincipal[0].Substring(product.CodigoPrincipal[0].IndexOf(":") + 2);

        string? imageUrl = product.Imagenes?
            .OrderBy(x => x.Orden)
            .Select(x => $"https://imagenes.compragamer.com/productos/compragamer_Imganen_general_{x.Nombre}.jpg")
            .FirstOrDefault();

        string productUrl = $"https://compragamer.com/producto/{product.Nombre.Replace(' ', '_')}__{product.IdProducto}";

        var productDetails = new ProductDetails(product.Nombre, productUrl, imageUrl, product.PrecioEspecial);
        var @event = new ProductScrapped(sku, 1, productDetails, DateOnly.FromDateTime(DateTime.UtcNow));

        _logger.LogDebug("Notifying product {ProductId} {ProductSKU}", product.IdProducto, sku);
        await _publishEndpoint.Publish(@event);
    }
}
