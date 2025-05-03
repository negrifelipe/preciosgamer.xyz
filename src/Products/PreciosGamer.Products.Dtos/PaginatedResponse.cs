namespace PreciosGamer.Products.Dtos;

public class PaginatedResponse<T>
{
    public List<T> Items { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }

    public PaginatedResponse(List<T> items, int page, int pageSize, int totalItemsCount)
    {
        Items = items;
        Page = page;
        PageSize = pageSize;
        HasNextPage = totalItemsCount > (page * pageSize) + items.Count;
        HasPreviousPage = page > 0;
    }
}
