using PreciosGamer.Products.CompraGamer.Models;
using System.Net.Http.Json;

namespace PreciosGamer.Products.CompraGamer.Services;

public class CompraGamerProductFetcherService 
{
    public async Task<IEnumerable<CompraGamerProduct>> FetchProducts()
    {
        var client = new HttpClient();
        var response = await client.GetAsync("https://static.compragamer.com/productos");
        var products = await response.Content.ReadFromJsonAsync<CompraGamerProduct[]>() ?? throw new Exception("Fetched 0 products from Compra Gamer");
        return products;
    }
}
