namespace SportZone.Application.DTOs.ArticuloVariante;

public class CreateArticuloVarianteDto
{
    public int ArticuloId { get; set; }
    public string? TallaUs { get; set; }
    public string? TallaEu { get; set; }
    public string? TallaUk { get; set; }
    public string? TallaCm { get; set; }
    public string? Color { get; set; }
    public string? CodigoBarras { get; set; }
    public int Stock { get; set; }
    public int StockMinimo { get; set; } = 5;
    public decimal? PrecioVentaOverride { get; set; }
}
