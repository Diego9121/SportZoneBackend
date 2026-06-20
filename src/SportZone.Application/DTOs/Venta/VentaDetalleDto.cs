namespace SportZone.Application.DTOs.Venta;

public class VentaDetalleDto
{
    public int Id { get; set; }
    public int VarianteId { get; set; }
    public string VarianteDescripcion { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Descuento { get; set; }
    public decimal Subtotal { get; set; }
}
