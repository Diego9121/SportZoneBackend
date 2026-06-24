using SportZone.Application.DTOs.Reporte;
using SportZone.Application.Interfaces;
using SportZone.Application.Interfaces.Servicios;

namespace SportZone.Application.Servicios;

public class ReporteServicio : IReporteServicio
{
    private readonly IReporteRepository _repository;

    public ReporteServicio(IReporteRepository repository)
    {
        _repository = repository;
    }

    public Task<ReporteVentasResumenDto> ObtenerResumenVentasAsync(DateTime? desde, DateTime? hasta)
    {
        return _repository.ObtenerResumenVentasAsync(desde, hasta);
    }

    public Task<List<ReporteArticuloVendidoDto>> ObtenerTopArticulosVendidosAsync(DateTime? desde, DateTime? hasta, int top)
    {
        var topValido = top <= 0 ? 10 : Math.Min(top, 100);
        return _repository.ObtenerTopArticulosVendidosAsync(desde, hasta, topValido);
    }

    public Task<ReporteComprasResumenDto> ObtenerResumenComprasAsync(DateTime? desde, DateTime? hasta)
    {
        return _repository.ObtenerResumenComprasAsync(desde, hasta);
    }

    public Task<List<ReporteComprasPorProveedorDto>> ObtenerComprasPorProveedorAsync(DateTime? desde, DateTime? hasta)
    {
        return _repository.ObtenerComprasPorProveedorAsync(desde, hasta);
    }

    public Task<ReporteVentasDetalladoDto> ObtenerReporteVentasAsync(DateTime? desde, DateTime? hasta)
    {
        return _repository.ObtenerReporteVentasAsync(desde, hasta);
    }

    public Task<ReporteComprasDetalladoDto> ObtenerReporteComprasAsync(DateTime? desde, DateTime? hasta)
    {
        return _repository.ObtenerReporteComprasAsync(desde, hasta);
    }
}
