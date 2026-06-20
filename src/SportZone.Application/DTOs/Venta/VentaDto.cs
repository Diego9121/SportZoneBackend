namespace SportZone.Application.DTOs.Venta;

public class VentaDto
{
    public int Id { get; set; }
    public int? ClienteId { get; set; }
    public string? ClienteNombre { get; set; }
    public string NumeroDoc { get; set; } = string.Empty;
    public string TipoComprobante { get; set; } = string.Empty;
    public decimal Subtotal { get; set; }
    public decimal Descuento { get; set; }
    public decimal Total { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string? Observacion { get; set; }
    public List<VentaDetalleDto> Detalles { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}
