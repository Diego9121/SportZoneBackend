namespace SportZone.Application.DTOs.Reporte;

// GananciaBruta = TotalVendido - TotalCosto. Se puede calcular con exactitud porque cada VentaDetalle
// guarda el PrecioCosto de la variante vigente al momento de la venta (no el costo actual, que pudo cambiar).
public class ReporteVentasResumenDto
{
    public DateTime? Desde { get; set; }
    public DateTime? Hasta { get; set; }
    public int CantidadVentas { get; set; }
    public decimal TotalVendido { get; set; }
    public decimal TotalDescuentos { get; set; }
    public decimal TotalCosto { get; set; }
    public decimal GananciaBruta { get; set; }
    public decimal TicketPromedio { get; set; }
}

// Ranking de variantes más vendidas en el período. GananciaGenerada usa el mismo criterio que el resumen.
public class ReporteArticuloVendidoDto
{
    public int VarianteId { get; set; }
    public int ArticuloId { get; set; }
    public string ArticuloNombre { get; set; } = string.Empty;
    public string VarianteDescripcion { get; set; } = string.Empty;
    public int CantidadVendida { get; set; }
    public decimal TotalVendido { get; set; }
    public decimal GananciaGenerada { get; set; }
}

// Listado completo de ventas del período (incluye anuladas, identificables por Estado) + el resumen agregado
public class ReporteVentasDetalladoDto
{
    public ReporteVentasResumenDto Resumen { get; set; } = new();
    public List<ReporteVentaItemDto> Ventas { get; set; } = new();
}

public class ReporteVentaItemDto
{
    public int Id { get; set; }
    public string NumeroDoc { get; set; } = string.Empty;
    public string TipoComprobante { get; set; } = string.Empty;
    public string? ClienteNombre { get; set; }
    public string Estado { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public decimal Descuento { get; set; }
    public decimal Costo { get; set; }
    public decimal Ganancia { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<ReporteVentaDetalleItemDto> Detalles { get; set; } = new();
}

public class ReporteVentaDetalleItemDto
{
    public string Nombre { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Total { get; set; }
}
