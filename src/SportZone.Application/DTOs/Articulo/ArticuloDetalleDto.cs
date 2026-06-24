namespace SportZone.Application.DTOs.Articulo;

// DTO de salida exclusivo para GET por Id: SI incluye el detalle completo de variantes.
// El listado (GetAll) usa ArticuloDto, que NO trae este detalle por rendimiento.
public class ArticuloDetalleDto
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
    public int TotalVariantes { get; set; }
    public int StockTotal { get; set; }
    public List<ArticuloVarianteResumenDto> Variantes { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

// Versión liviana de la variante para anidar aquí: no repite ArticuloId/ArticuloNombre/ArticuloCodigo
// porque ya estamos dentro del Articulo (eso sí tiene sentido en ArticuloVarianteDto, que se usa suelto).
public class ArticuloVarianteResumenDto
{
    public int Id { get; set; }
    public string? TallaUs { get; set; }
    public string? TallaEu { get; set; }
    public string? TallaUk { get; set; }
    public string? TallaCm { get; set; }
    public string? Color { get; set; }
    public string? CodigoBarras { get; set; }
    public string? ArticuloImagen { get; set; }
    public string? ImagenUrl { get; set; }
    public int Stock { get; set; }
    public int StockMinimo { get; set; }
    public bool StockBajo { get; set; }
    public decimal PrecioVenta { get; set; }
    public decimal PrecioCosto { get; set; }
}
