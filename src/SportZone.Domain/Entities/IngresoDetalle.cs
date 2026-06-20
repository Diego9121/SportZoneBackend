namespace SportZone.Domain.Entities;

public class IngresoDetalle : BaseEntity
{
    public int IngresoId { get; set; }
    public int VarianteId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioCosto { get; set; }
    public decimal Subtotal { get; set; }

    public Ingreso Ingreso { get; set; } = null!;
    public ArticuloVariante Variante { get; set; } = null!;
}
