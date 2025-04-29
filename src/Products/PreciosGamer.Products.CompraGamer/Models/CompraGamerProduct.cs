using System.Text.Json.Serialization;

namespace PreciosGamer.Products.CompraGamer.Models;

public class CompraGamerProduct
{
    [JsonPropertyName("id_producto")]
    public int IdProducto { get; set; }

    [JsonPropertyName("nombre")]
    public string Nombre { get; set; } = null!;

    [JsonPropertyName("id_categoria")]
    public int IdCategoria { get; set; }

    [JsonPropertyName("precioEspecial")]
    public int PrecioEspecial { get; set; }

    [JsonPropertyName("imagenes")]
    public Imagen[]? Imagenes { get; set; } = Array.Empty<Imagen>();

    [JsonPropertyName("codigo_principal")]
    public string[]? CodigoPrincipal { get; set; } = Array.Empty<string>();

    public class Imagen
    {
        [JsonPropertyName("nombre")]
        public string Nombre { get; set; } = null!;

        [JsonPropertyName("orden")]
        public int Orden { get; set; }
    }
}
