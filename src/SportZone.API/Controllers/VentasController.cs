using SportZone.Application.DTOs.Common;
using SportZone.Application.DTOs.Venta;
using SportZone.Application.Interfaces.Servicios;

namespace SportZone.API.Controllers;

[Route("api/[controller]")]
public class VentasController : BaseController
{
    private readonly IVentaServicio _servicio;

    public VentasController(IVentaServicio servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PaginacionQueryDto query)
    {
        var resultado = await _servicio.GetAllAsync(query);
        return RespuestaOk(resultado);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var venta = await _servicio.GetByIdAsync(id);
        return venta == null ? NotFound() : RespuestaOk(venta);
    }

    // Decrementa stock, valida pagos mixtos y acumula puntos de fidelizacion si hay Cliente
    [HttpPost]
    public async Task<IActionResult> Create(CreateVentaDto dto)
    {
        var creada = await _servicio.CreateAsync(dto);
        return RespuestaCreado(creada, "Venta registrada");
    }

    // Reintegra el stock vendido y marca la venta como ANULADA
    [HttpPut("{id:int}/anular")]
    public async Task<IActionResult> Anular(int id)
    {
        var anulada = await _servicio.AnularAsync(id);
        return RespuestaOk(anulada, "Venta anulada y stock reintegrado");
    }
}
