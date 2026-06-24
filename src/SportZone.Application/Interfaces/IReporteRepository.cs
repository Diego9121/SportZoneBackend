using SportZone.Application.DTOs.Reporte;

namespace SportZone.Application.Interfaces;

public interface IReporteRepository
{
    Task<ReporteVentasResumenDto> ObtenerResumenVentasAsync(DateTime? desde, DateTime? hasta);
    Task<List<ReporteArticuloVendidoDto>> ObtenerTopArticulosVendidosAsync(DateTime? desde, DateTime? hasta, int top);
    Task<ReporteComprasResumenDto> ObtenerResumenComprasAsync(DateTime? desde, DateTime? hasta);
    Task<List<ReporteComprasPorProveedorDto>> ObtenerComprasPorProveedorAsync(DateTime? desde, DateTime? hasta);
    Task<ReporteVentasDetalladoDto> ObtenerReporteVentasAsync(DateTime? desde, DateTime? hasta);
    Task<ReporteComprasDetalladoDto> ObtenerReporteComprasAsync(DateTime? desde, DateTime? hasta);
}
