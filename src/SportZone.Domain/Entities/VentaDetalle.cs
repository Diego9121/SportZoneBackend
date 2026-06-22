namespace SportZone.Domain.Entities;

public class VentaDetalle : BaseEntity
{
    public int VentaId { get; set; }
    public int VarianteId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }

    // Costo de la variante al momento de la venta (copiado de ArticuloVariante.PrecioCosto al crear la venta).
    // Se guarda aquí, no se calcula al vuelo, porque el costo de la variante puede cambiar después
    // (nuevos ingresos) y la ganancia de una venta pasada debe quedar fija con el costo de ESE momento.
    public decimal PrecioCosto { get; set; }

    public decimal Descuento { get; set; } = 0;
    public decimal Subtotal { get; set; }

    public Venta Venta { get; set; } = null!;
    public ArticuloVariante Variante { get; set; } = null!;
}
