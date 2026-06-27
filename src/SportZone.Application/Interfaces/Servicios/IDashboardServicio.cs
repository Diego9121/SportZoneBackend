using SportZone.Application.DTOs.Dashboard;
using SportZone.Application.DTOs.Reporte;

namespace SportZone.Application.Interfaces.Servicios;

// Todo es solo lectura y se basa en datos que ya generan Venta/Ingreso (ver IReporteServicio).
// anio/mes son opcionales en todos: si no se envian, se usa el mes actual.
public interface IDashboardServicio
{
    Task<DashboardVentasMesDto> ObtenerVentasMesAsync(int? anio, int? mes);
    Task<DashboardComprasMesDto> ObtenerComprasMesAsync(int? anio, int? mes);
    Task<DashboardMargenGananciaMesDto> ObtenerMargenGananciaMesAsync(int? anio, int? mes);
    Task<DashboardParesVendidosMesDto> ObtenerParesVendidosMesAsync(int? anio, int? mes);
    Task<List<DashboardVentasPorCategoriaDto>> ObtenerVentasPorCategoriaMesAsync(int? anio, int? mes);
    Task<List<ReporteArticuloVendidoDto>> ObtenerTopProductosMesAsync(int? anio, int? mes, int top);
    Task<List<DashboardSerieDiariaDto>> ObtenerComprasVsVentasMesAsync(int? anio, int? mes);
}
