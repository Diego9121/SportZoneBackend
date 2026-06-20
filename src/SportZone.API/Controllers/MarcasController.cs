using SportZone.Application.DTOs.Common;
using SportZone.Application.DTOs.Marca;
using SportZone.Application.Interfaces.Servicios;

namespace SportZone.API.Controllers;

[Route("api/[controller]")]
public class MarcasController : BaseController
{
    private readonly IMarcaServicio _servicio;

    public MarcasController(IMarcaServicio servicio)
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
        var marca = await _servicio.GetByIdAsync(id);
        return marca == null ? NotFound() : RespuestaOk(marca);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateMarcaDto dto)
    {
        var creada = await _servicio.CreateAsync(dto);
        return RespuestaCreado(creada, "Marca creada");
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateMarcaDto dto)
    {
        var actualizada = await _servicio.UpdateAsync(id, dto);
        return RespuestaOk(actualizada, "Marca actualizada");
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _servicio.DeleteAsync(id);
        return RespuestaSinDatos("Marca eliminada");
    }
}
