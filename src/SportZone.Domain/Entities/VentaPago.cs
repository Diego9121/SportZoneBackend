namespace SportZone.Domain.Entities;

public class VentaPago : BaseEntity
{
    public int VentaId { get; set; }
    public string MetodoPago { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public string? Referencia { get; set; }

    public Venta Venta { get; set; } = null!;
}
