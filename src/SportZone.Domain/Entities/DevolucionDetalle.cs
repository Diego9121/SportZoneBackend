namespace SportZone.Domain.Entities;

public class DevolucionDetalle : BaseEntity
{
    public int DevolucionId { get; set; }
    public int VarianteId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }

    public Devolucion Devolucion { get; set; } = null!;
    public ArticuloVariante Variante { get; set; } = null!;
}
