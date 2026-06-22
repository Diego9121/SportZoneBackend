namespace SportZone.Application.DTOs.ArticuloVariante;

// DTO enriquecido para catálogo/POS/consulta móvil: agrega el nombre completo del Articulo y la URL
// de su imagen, algo que ArticuloVarianteDto (el de CRUD normal) no trae.
public class ArticuloVarianteCatalogoDto
{
    public int Id { get; set; }
    public int ArticuloId { get; set; }
    public string ArticuloNombre { get; set; } = string.Empty;
    public string? ArticuloImagen { get; set; }
    public string? TallaUs { get; set; }
    public string? TallaEu { get; set; }
    public string? TallaUk { get; set; }
    public string? TallaCm { get; set; }
    public string? Color { get; set; }
    public string? CodigoBarras { get; set; }
    public int Stock { get; set; }
    public decimal PrecioVenta { get; set; }
    public decimal PrecioCosto { get; set; }
}
