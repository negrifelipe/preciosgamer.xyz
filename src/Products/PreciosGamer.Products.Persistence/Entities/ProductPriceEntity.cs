namespace PreciosGamer.Products.Persistence.Entities;

public class ProductPriceEntity
{
    public string SKU { get; set; } = string.Empty;
    public int StoreId { get; set; }
    public decimal Price { get; set; }
    public DateOnly CreateDate { get; set; }
}
