using SportZone.Application.DTOs.Reporte;

namespace SportZone.Application.Interfaces.Servicios;

// Solo lectura: un reporte no se "crea", se calcula sobre datos que ya generaron Venta e Ingreso.
public interface IReporteServicio
{
    Task<ReporteVentasResumenDto> ObtenerResumenVentasAsync(DateTime? desde, DateTime? hasta);
    Task<List<ReporteArticuloVendidoDto>> ObtenerTopArticulosVendidosAsync(DateTime? desde, DateTime? hasta, int top);
    Task<ReporteComprasResumenDto> ObtenerResumenComprasAsync(DateTime? desde, DateTime? hasta);
    Task<List<ReporteComprasPorProveedorDto>> ObtenerComprasPorProveedorAsync(DateTime? desde, DateTime? hasta);
    Task<ReporteVentasDetalladoDto> ObtenerReporteVentasAsync(DateTime? desde, DateTime? hasta);
    Task<ReporteComprasDetalladoDto> ObtenerReporteComprasAsync(DateTime? desde, DateTime? hasta);
}
