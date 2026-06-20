namespace SportZone.Domain.Entities;

public class Venta : BaseEntity
{
    public int? ClienteId { get; set; }
    public int UsuarioId { get; set; }
    public string NumeroDoc { get; set; } = string.Empty;
    public string TipoComprobante { get; set; } = "RECIBO";
    public decimal Subtotal { get; set; }
    public decimal Descuento { get; set; } = 0;
    public decimal Total { get; set; }
    public string Estado { get; set; } = "PENDIENTE";
    public string? Observacion { get; set; }

    public Cliente? Cliente { get; set; }
    public Usuario Usuario { get; set; } = null!;

    public ICollection<VentaDetalle> VentaDetalles { get; set; } = new List<VentaDetalle>();
    public ICollection<VentaPago> VentaPagos { get; set; } = new List<VentaPago>();
    public ICollection<Devolucion> Devoluciones { get; set; } = new List<Devolucion>();
}
