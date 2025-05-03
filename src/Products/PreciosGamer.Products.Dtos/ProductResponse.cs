namespace PreciosGamer.Products.Dtos;

public record ProductResponse(string SKU, int StoreId, DateOnly CreateDate, ProductDetails Details);
