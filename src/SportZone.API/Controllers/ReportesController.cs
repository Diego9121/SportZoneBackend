using SportZone.Application.Interfaces.Servicios;

namespace SportZone.API.Controllers;

// Solo lectura: agrega datos ya generados por Ventas e Ingresos, no crea ni modifica nada.
[Route("api/[controller]")]
public class ReportesController : BaseController
{
    private readonly IReporteServicio _servicio;

    public ReportesController(IReporteServicio servicio)
    {
        _servicio = servicio;
    }

    // GET /api/Reportes/ventas/resumen?desde=2026-06-01&hasta=2026-06-23
    [HttpGet("ventas/resumen")]
    public async Task<IActionResult> GetResumenVentas([FromQuery] DateTime? desde, [FromQuery] DateTime? hasta)
    {
        var resultado = await _servicio.ObtenerResumenVentasAsync(desde, hasta);
        return RespuestaOk(resultado);
    }

    // GET /api/Reportes/ventas/top-articulos?desde=&hasta=&top=10
    [HttpGet("ventas/top-articulos")]
    public async Task<IActionResult> GetTopArticulosVendidos([FromQuery] DateTime? desde, [FromQuery] DateTime? hasta, [FromQuery] int top = 10)
    {
        var resultado = await _servicio.ObtenerTopArticulosVendidosAsync(desde, hasta, top);
        return RespuestaOk(resultado);
    }

    // GET /api/Reportes/compras/resumen?desde=&hasta=
    [HttpGet("compras/resumen")]
    public async Task<IActionResult> GetResumenCompras([FromQuery] DateTime? desde, [FromQuery] DateTime? hasta)
    {
        var resultado = await _servicio.ObtenerResumenComprasAsync(desde, hasta);
        return RespuestaOk(resultado);
    }

    // GET /api/Reportes/compras/por-proveedor?desde=&hasta=
    [HttpGet("compras/por-proveedor")]
    public async Task<IActionResult> GetComprasPorProveedor([FromQuery] DateTime? desde, [FromQuery] DateTime? hasta)
    {
        var resultado = await _servicio.ObtenerComprasPorProveedorAsync(desde, hasta);
        return RespuestaOk(resultado);
    }

    // GET /api/Reportes/ventas?desde=&hasta=  ->  listado completo de ventas del período + su resumen
    [HttpGet("ventas")]
    public async Task<IActionResult> GetReporteVentas([FromQuery] DateTime? desde, [FromQuery] DateTime? hasta)
    {
        var resultado = await _servicio.ObtenerReporteVentasAsync(desde, hasta);
        return RespuestaOk(resultado);
    }

    // GET /api/Reportes/compras?desde=&hasta=  ->  listado completo de compras del período + su resumen
    [HttpGet("compras")]
    public async Task<IActionResult> GetReporteCompras([FromQuery] DateTime? desde, [FromQuery] DateTime? hasta)
    {
        var resultado = await _servicio.ObtenerReporteComprasAsync(desde, hasta);
        return RespuestaOk(resultado);
    }
}
