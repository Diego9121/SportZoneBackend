namespace SportZone.Domain.Entities;

public class ArticuloVariante : BaseEntity
{
    public int ArticuloId { get; set; }
    public string? TallaUs { get; set; }
    public string? TallaEu { get; set; }
    public string? TallaUk { get; set; }
    public string? TallaCm { get; set; }
    public string? Color { get; set; }
    public string? CodigoBarras { get; set; }
    public int Stock { get; set; } = 0;
    public int StockMinimo { get; set; } = 5;
    public decimal? PrecioVentaOverride { get; set; }

    public Articulo Articulo { get; set; } = null!;

    public ICollection<IngresoDetalle> IngresoDetalles { get; set; } = new List<IngresoDetalle>();
    public ICollection<VentaDetalle> VentaDetalles { get; set; } = new List<VentaDetalle>();
    public ICollection<DevolucionDetalle> DevolucionDetalles { get; set; } = new List<DevolucionDetalle>();
}
