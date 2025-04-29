namespace PreciosGamer.Products.Dtos.Events;

public record ProductScrapped(string SKU, int StoreId, ProductDetails Details, DateOnly ScrappedDate);
