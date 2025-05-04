namespace PreciosGamer.Products.Dtos;

public record ProductPriceResponse(string ProductSKU, int StoreId, DateOnly PriceDate, decimal Price);
