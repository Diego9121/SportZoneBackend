namespace SportZone.Domain.Entities;

// Ya no referencia Usuario directamente: CreateById (heredado de BaseEntity) ya identifica quién la registró.
// Tampoco maneja pagos mixtos (VentaPago se eliminó del esquema).
public class Venta : BaseEntity
{
    public int? ClienteId { get; set; }
    public string NumeroDoc { get; set; } = string.Empty;
    public string TipoComprobante { get; set; } = "RECIBO";
    public decimal Subtotal { get; set; }
    public decimal Descuento { get; set; } = 0;
    public decimal Total { get; set; }
    public string Estado { get; set; } = "PENDIENTE";
    public string? Observacion { get; set; }

    public Cliente? Cliente { get; set; }

    public ICollection<VentaDetalle> VentaDetalles { get; set; } = new List<VentaDetalle>();
    public ICollection<MovimientoStock> MovimientosStock { get; set; } = new List<MovimientoStock>();
}
