using SportZone.Application.DTOs.Common;
using SportZone.Application.DTOs.Rol;
using SportZone.Application.Interfaces.Servicios;

namespace SportZone.API.Controllers;

[Route("api/[controller]")]
public class RolesController : BaseController
{
    private readonly IRolServicio _servicio;

    public RolesController(IRolServicio servicio)
    {
        _servicio = servicio;
    }

    // GET /api/Roles?page=1&pageSize=10
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PaginacionQueryDto query)
    {
        var resultado = await _servicio.GetAllAsync(query);
        return RespuestaOk(resultado);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var rol = await _servicio.GetByIdAsync(id);
        return rol == null ? NotFound() : RespuestaOk(rol);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateRolDto dto)
    {
        var creado = await _servicio.CreateAsync(dto);
        return RespuestaCreado(creado, "Rol creado");
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateRolDto dto)
    {
        var actualizado = await _servicio.UpdateAsync(id, dto);
        return RespuestaOk(actualizado, "Rol actualizado");
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _servicio.DeleteAsync(id);
        return RespuestaSinDatos("Rol eliminado");
    }
}
