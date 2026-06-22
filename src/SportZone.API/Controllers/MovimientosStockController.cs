using SportZone.Application.DTOs.Common;
using SportZone.Application.Interfaces.Servicios;

namespace SportZone.API.Controllers;

// Solo lectura: los movimientos los genera automáticamente Ingreso/Venta, no se crean a mano aquí.
[Route("api/[controller]")]
public class MovimientosStockController : BaseController
{
    private readonly IMovimientoStockServicio _servicio;

    public MovimientosStockController(IMovimientoStockServicio servicio)
    {
        _servicio = servicio;
    }

    // GET /api/MovimientosStock?page=1&pageSize=10&articuloVarianteId=5
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PaginacionQueryDto query, [FromQuery] int? articuloVarianteId)
    {
        var resultado = await _servicio.GetAllAsync(query, articuloVarianteId);
        return RespuestaOk(resultado);
    }

    // GET /api/MovimientosStock/5  ->  5 aquí es el Id de la VARIANTE, no el del movimiento.
    // Devuelve TODO el historial de movimientos de esa variante (puede ser una lista vacía).
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetByVarianteId(int id)
    {
        var resultado = await _servicio.GetByVarianteIdAsync(id);
        return RespuestaOk(resultado);
    }
}
