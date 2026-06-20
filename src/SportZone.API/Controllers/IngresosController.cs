using SportZone.Application.DTOs.Common;
using SportZone.Application.DTOs.Ingreso;
using SportZone.Application.Interfaces.Servicios;

namespace SportZone.API.Controllers;

[Route("api/[controller]")]
public class IngresosController : BaseController
{
    private readonly IIngresoServicio _servicio;

    public IngresosController(IIngresoServicio servicio)
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
        var ingreso = await _servicio.GetByIdAsync(id);
        return ingreso == null ? NotFound() : RespuestaOk(ingreso);
    }

    // Al crear, incrementa automáticamente el stock de cada variante incluida en el detalle
    [HttpPost]
    public async Task<IActionResult> Create(CreateIngresoDto dto)
    {
        var creado = await _servicio.CreateAsync(dto);
        return RespuestaCreado(creado, "Ingreso registrado");
    }
}
