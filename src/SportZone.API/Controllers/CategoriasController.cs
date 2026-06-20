using SportZone.Application.DTOs.Categoria;
using SportZone.Application.DTOs.Common;
using SportZone.Application.Interfaces.Servicios;

namespace SportZone.API.Controllers;

[Route("api/[controller]")]
public class CategoriasController : BaseController
{
    private readonly ICategoriaServicio _servicio;

    public CategoriasController(ICategoriaServicio servicio)
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
        var categoria = await _servicio.GetByIdAsync(id);
        return categoria == null ? NotFound() : RespuestaOk(categoria);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCategoriaDto dto)
    {
        var creada = await _servicio.CreateAsync(dto);
        return RespuestaCreado(creada, "Categoria creada");
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateCategoriaDto dto)
    {
        var actualizada = await _servicio.UpdateAsync(id, dto);
        return RespuestaOk(actualizada, "Categoria actualizada");
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _servicio.DeleteAsync(id);
        return RespuestaSinDatos("Categoria eliminada");
    }
}
