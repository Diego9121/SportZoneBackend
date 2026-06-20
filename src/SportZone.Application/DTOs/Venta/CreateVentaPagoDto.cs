namespace SportZone.Application.DTOs.Venta;

public class CreateVentaPagoDto
{
    public string MetodoPago { get; set; } = string.Empty; // EFECTIVO | TARJETA_DEBITO | TARJETA_CREDITO | PAGO_MOVIL
    public decimal Monto { get; set; }
    public string? Referencia { get; set; }
}
