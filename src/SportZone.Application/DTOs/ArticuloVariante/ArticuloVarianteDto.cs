namespace SportZone.Application.DTOs.ArticuloVariante;

// StockBajo es calculado: nunca se guarda en la base de datos.
public class ArticuloVarianteDto
{
    public int Id { get; set; }
    public int ArticuloId { get; set; }
    public string ArticuloNombre { get; set; } = string.Empty;
    public string ArticuloCodigo { get; set; } = string.Empty;
    public string? TallaUs { get; set; }
    public string? TallaEu { get; set; }
    public string? TallaUk { get; set; }
    public string? TallaCm { get; set; }
    public string? Color { get; set; }
    public string? CodigoBarras { get; set; }
    public int Stock { get; set; }
    public int StockMinimo { get; set; }
    public bool StockBajo { get; set; }
    public decimal PrecioVenta { get; set; }
    public decimal PrecioCosto { get; set; }
    public DateTime CreatedAt { get; set; }
}
