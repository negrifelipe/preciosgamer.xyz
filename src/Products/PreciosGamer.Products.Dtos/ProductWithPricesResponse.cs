
namespace PreciosGamer.Products.Dtos;

public record ProductWithPricesResponse : ProductResponse
{
    public List<ProductPriceResponse> Prices { get; init; }

    public ProductWithPricesResponse(string SKU, int StoreId, DateOnly CreateDate, ProductDetails Details, List<ProductPriceResponse> Prices) : base(SKU, StoreId, CreateDate, Details)
    {
        this.Prices = Prices;
    }
}
