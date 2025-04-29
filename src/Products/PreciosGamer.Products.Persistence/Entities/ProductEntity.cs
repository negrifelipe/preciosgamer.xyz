using NpgsqlTypes;

namespace PreciosGamer.Products.Persistence.Entities;

public class ProductEntity
{
    public string SKU { get; set; } = string.Empty;
    public int StoreId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public decimal Price { get; set; }
    public DateOnly CreateDate { get; set; }
    public NpgsqlTsVector SearchVector { get; set; } = null!;
}