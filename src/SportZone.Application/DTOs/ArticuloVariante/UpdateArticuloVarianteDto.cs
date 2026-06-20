namespace SportZone.Application.DTOs.ArticuloVariante;

// No incluye Stock a propósito: el stock solo cambia vía Ingreso/Venta o el endpoint AjustarStock.
public class UpdateArticuloVarianteDto
{
    public string? TallaUs { get; set; }
    public string? TallaEu { get; set; }
    public string? TallaUk { get; set; }
    public string? TallaCm { get; set; }
    public string? Color { get; set; }
    public string? CodigoBarras { get; set; }
    public int StockMinimo { get; set; }
    public decimal PrecioVenta { get; set; }
    public decimal PrecioCosto { get; set; }
}
