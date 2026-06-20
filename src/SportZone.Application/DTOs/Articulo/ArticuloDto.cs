namespace SportZone.Application.DTOs.Articulo;

// DTO de salida: CategoriaNombre/MarcaNombre vienen aplanados desde sus relaciones.
// StockTotal y TotalVariantes son calculados a partir de las variantes cargadas.
public class ArticuloDto
{
    public int Id { get; set; }
    public int CategoriaId { get; set; }
    public string CategoriaNombre { get; set; } = string.Empty;
    public int MarcaId { get; set; }
    public string MarcaNombre { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? Imagen { get; set; }
    public decimal PrecioVenta { get; set; }
    public decimal PrecioCosto { get; set; }
    public int TotalVariantes { get; set; }
    public int StockTotal { get; set; }
    public DateTime CreatedAt { get; set; }
}
