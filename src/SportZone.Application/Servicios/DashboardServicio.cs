using SportZone.Application.DTOs.Dashboard;
using SportZone.Application.DTOs.Reporte;
using SportZone.Application.Interfaces;
using SportZone.Application.Interfaces.Servicios;

namespace SportZone.Application.Servicios;

public class DashboardServicio : IDashboardServicio
{
    private readonly IReporteRepository _reporteRepository;
    private readonly IDashboardRepository _dashboardRepository;

    public DashboardServicio(IReporteRepository reporteRepository, IDashboardRepository dashboardRepository)
    {
        _reporteRepository = reporteRepository;
        _dashboardRepository = dashboardRepository;
    }

    public async Task<DashboardVentasMesDto> ObtenerVentasMesAsync(int? anio, int? mes)
    {
        var rango = CalcularRangoMes(anio, mes);

        var resumenActual = await _reporteRepository.ObtenerResumenVentasAsync(rango.InicioActual, rango.FinActual);
        var resumenAnterior = await _reporteRepository.ObtenerResumenVentasAsync(rango.InicioAnterior, rango.FinAnterior);

        return new DashboardVentasMesDto
        {
            Anio = rango.Anio,
            Mes = rango.Mes,
            TotalMesActual = resumenActual.TotalVendido,
            TotalMesAnterior = resumenAnterior.TotalVendido,
            PorcentajeCambio = CalcularPorcentajeCambio(resumenActual.TotalVendido, resumenAnterior.TotalVendido)
        };
    }

    public async Task<DashboardComprasMesDto> ObtenerComprasMesAsync(int? anio, int? mes)
    {
        var rango = CalcularRangoMes(anio, mes);

        var resumenActual = await _reporteRepository.ObtenerResumenComprasAsync(rango.InicioActual, rango.FinActual);
        var resumenAnterior = await _reporteRepository.ObtenerResumenComprasAsync(rango.InicioAnterior, rango.FinAnterior);

        return new DashboardComprasMesDto
        {
            Anio = rango.Anio,
            Mes = rango.Mes,
            TotalMesActual = resumenActual.TotalComprado,
            TotalMesAnterior = resumenAnterior.TotalComprado,
            PorcentajeCambio = CalcularPorcentajeCambio(resumenActual.TotalComprado, resumenAnterior.TotalComprado)
        };
    }

    public async Task<DashboardMargenGananciaMesDto> ObtenerMargenGananciaMesAsync(int? anio, int? mes)
    {
        var rango = CalcularRangoMes(anio, mes);
        var resumen = await _reporteRepository.ObtenerResumenVentasAsync(rango.InicioActual, rango.FinActual);

        return new DashboardMargenGananciaMesDto
        {
            Anio = rango.Anio,
            Mes = rango.Mes,
            TotalVendido = resumen.TotalVendido,
            TotalCosto = resumen.TotalCosto,
            GananciaBruta = resumen.GananciaBruta,
            MargenPorcentaje = resumen.TotalVendido == 0
                ? 0
                : Math.Round((double)(resumen.GananciaBruta / resumen.TotalVendido * 100), 2)
        };
    }

    public async Task<DashboardParesVendidosMesDto> ObtenerParesVendidosMesAsync(int? anio, int? mes)
    {
        var rango = CalcularRangoMes(anio, mes);
        var pares = await _dashboardRepository.ObtenerParesVendidosAsync(rango.InicioActual, rango.FinActual);

        return new DashboardParesVendidosMesDto
        {
            Anio = rango.Anio,
            Mes = rango.Mes,
            ParesVendidos = pares
        };
    }

    public async Task<List<DashboardVentasPorCategoriaDto>> ObtenerVentasPorCategoriaMesAsync(int? anio, int? mes)
    {
        var rango = CalcularRangoMes(anio, mes);
        return await _dashboardRepository.ObtenerVentasPorCategoriaAsync(rango.InicioActual, rango.FinActual);
    }

    public async Task<List<ReporteArticuloVendidoDto>> ObtenerTopProductosMesAsync(int? anio, int? mes, int top)
    {
        var rango = CalcularRangoMes(anio, mes);
        var topValido = top <= 0 ? 5 : Math.Min(top, 100);
        return await _reporteRepository.ObtenerTopArticulosVendidosAsync(rango.InicioActual, rango.FinActual, topValido);
    }

    public async Task<List<DashboardSerieDiariaDto>> ObtenerComprasVsVentasMesAsync(int? anio, int? mes)
    {
        var rango = CalcularRangoMes(anio, mes);
        return await _dashboardRepository.ObtenerSerieDiariaAsync(rango.InicioActual, rango.FinActual);
    }

    private static double CalcularPorcentajeCambio(decimal actual, decimal anterior)
    {
        if (anterior == 0) return actual == 0 ? 0 : 100;
        return Math.Round((double)((actual - anterior) / anterior * 100), 2);
    }

    private static RangoMes CalcularRangoMes(int? anio, int? mes)
    {
        var hoy = DateTime.UtcNow;
        var anioFinal = anio ?? hoy.Year;
        var mesFinal = mes ?? hoy.Month;

        var inicioActual = new DateTime(anioFinal, mesFinal, 1);
        var finActual = inicioActual.AddMonths(1).AddTicks(-1);
        var inicioAnterior = inicioActual.AddMonths(-1);
        var finAnterior = inicioActual.AddTicks(-1);

        return new RangoMes(inicioActual, finActual, inicioAnterior, finAnterior, anioFinal, mesFinal);
    }

    private record RangoMes(DateTime InicioActual, DateTime FinActual, DateTime InicioAnterior, DateTime FinAnterior, int Anio, int Mes);
}
