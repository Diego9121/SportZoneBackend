using SportZone.Application.Interfaces.Servicios;

namespace SportZone.API.Controllers;

// Solo lectura: agrega datos ya generados por Ventas e Ingresos, igual que ReportesController.
// anio/mes son opcionales en todas las rutas: si no se envian, se usa el mes actual.
[Route("api/[controller]")]
public class DashboardController : BaseController
{
    private readonly IDashboardServicio _servicio;

    public DashboardController(IDashboardServicio servicio)
    {
        _servicio = servicio;
    }

    // GET /api/Dashboard/ventas-mes?anio=&mes=
    [HttpGet("ventas-mes")]
    public async Task<IActionResult> GetVentasMes([FromQuery] int? anio, [FromQuery] int? mes)
    {
        var resultado = await _servicio.ObtenerVentasMesAsync(anio, mes);
        return RespuestaOk(resultado);
    }

    // GET /api/Dashboard/compras-mes?anio=&mes=
    [HttpGet("compras-mes")]
    public async Task<IActionResult> GetComprasMes([FromQuery] int? anio, [FromQuery] int? mes)
    {
        var resultado = await _servicio.ObtenerComprasMesAsync(anio, mes);
        return RespuestaOk(resultado);
    }

    // GET /api/Dashboard/margen-ganancia-mes?anio=&mes=
    [HttpGet("margen-ganancia-mes")]
    public async Task<IActionResult> GetMargenGananciaMes([FromQuery] int? anio, [FromQuery] int? mes)
    {
        var resultado = await _servicio.ObtenerMargenGananciaMesAsync(anio, mes);
        return RespuestaOk(resultado);
    }

    // GET /api/Dashboard/pares-vendidos-mes?anio=&mes=
    [HttpGet("pares-vendidos-mes")]
    public async Task<IActionResult> GetParesVendidosMes([FromQuery] int? anio, [FromQuery] int? mes)
    {
        var resultado = await _servicio.ObtenerParesVendidosMesAsync(anio, mes);
        return RespuestaOk(resultado);
    }

    // GET /api/Dashboard/ventas-por-categoria-mes?anio=&mes=
    [HttpGet("ventas-por-categoria-mes")]
    public async Task<IActionResult> GetVentasPorCategoriaMes([FromQuery] int? anio, [FromQuery] int? mes)
    {
        var resultado = await _servicio.ObtenerVentasPorCategoriaMesAsync(anio, mes);
        return RespuestaOk(resultado);
    }

    // GET /api/Dashboard/top-productos-mes?anio=&mes=&top=5
    [HttpGet("top-productos-mes")]
    public async Task<IActionResult> GetTopProductosMes([FromQuery] int? anio, [FromQuery] int? mes, [FromQuery] int top = 5)
    {
        var resultado = await _servicio.ObtenerTopProductosMesAsync(anio, mes, top);
        return RespuestaOk(resultado);
    }

    // GET /api/Dashboard/compras-vs-ventas-mes?anio=&mes=
    [HttpGet("compras-vs-ventas-mes")]
    public async Task<IActionResult> GetComprasVsVentasMes([FromQuery] int? anio, [FromQuery] int? mes)
    {
        var resultado = await _servicio.ObtenerComprasVsVentasMesAsync(anio, mes);
        return RespuestaOk(resultado);
    }
}
