using SportZone.Application.DTOs.Articulo;
using SportZone.Application.DTOs.Common;
using SportZone.Application.Interfaces.Servicios;

namespace SportZone.API.Controllers;

[Route("api/[controller]")]
public class ArticulosController : BaseController
{
    private readonly IArticuloServicio _servicio;

    public ArticulosController(IArticuloServicio servicio)
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
        var articulo = await _servicio.GetByIdAsync(id);
        return articulo == null ? NotFound() : RespuestaOk(articulo);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateArticuloDto dto)
    {
        var creado = await _servicio.CreateAsync(dto);
        return RespuestaCreado(creado, "Articulo creado");
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateArticuloDto dto)
    {
        var actualizado = await _servicio.UpdateAsync(id, dto);
        return RespuestaOk(actualizado, "Articulo actualizado");
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _servicio.DeleteAsync(id);
        return RespuestaSinDatos("Articulo eliminado");
    }
}
