using SportZone.Application.DTOs.Dashboard;

namespace SportZone.Application.Interfaces;

// Consultas crudas que el Reporte no cubre (Reporte trabaja por venta/ingreso, esto agrega por dia/categoria/pares).
// inicio/fin son siempre inclusivos, igual convencion que IReporteRepository.
public interface IDashboardRepository
{
    Task<int> ObtenerParesVendidosAsync(DateTime inicio, DateTime fin);
    Task<List<DashboardVentasPorCategoriaDto>> ObtenerVentasPorCategoriaAsync(DateTime inicio, DateTime fin);
    Task<List<DashboardSerieDiariaDto>> ObtenerSerieDiariaAsync(DateTime inicio, DateTime fin);
}
