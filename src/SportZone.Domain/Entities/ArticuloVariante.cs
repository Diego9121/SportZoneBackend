namespace SportZone.Domain.Entities;

// PrecioVenta/PrecioCosto ahora son obligatorios aquí (antes era un "override" opcional sobre el Articulo).
public class ArticuloVariante : BaseEntity
{
    public int ArticuloId { get; set; }
    public string? TallaUs { get; set; }
    public string? TallaEu { get; set; }
    public string? TallaUk { get; set; }
    public string? TallaCm { get; set; }
    public string? Color { get; set; }
    public string? CodigoBarras { get; set; }
    public string? ImagenUrl { get; set; }
    public int Stock { get; set; } = 0;
    public int StockMinimo { get; set; } = 5;
    public decimal PrecioVenta { get; set; }
    public decimal PrecioCosto { get; set; }

    public Articulo Articulo { get; set; } = null!;

    public ICollection<IngresoDetalle> IngresoDetalles { get; set; } = new List<IngresoDetalle>();
    public ICollection<VentaDetalle> VentaDetalles { get; set; } = new List<VentaDetalle>();
    public ICollection<MovimientoStock> MovimientosStock { get; set; } = new List<MovimientoStock>();
}
