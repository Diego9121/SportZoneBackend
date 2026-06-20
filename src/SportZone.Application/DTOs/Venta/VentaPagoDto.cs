namespace SportZone.Application.DTOs.Venta;

public class VentaPagoDto
{
    public int Id { get; set; }
    public string MetodoPago { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public string? Referencia { get; set; }
}
