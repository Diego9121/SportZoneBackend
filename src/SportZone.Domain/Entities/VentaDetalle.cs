namespace SportZone.Domain.Entities;

public class VentaDetalle : BaseEntity
{
    public int VentaId { get; set; }
    public int VarianteId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Descuento { get; set; } = 0;
    public decimal Subtotal { get; set; }

    public Venta Venta { get; set; } = null!;
    public ArticuloVariante Variante { get; set; } = null!;
}
