using SportZone.Application.DTOs.Common;
using SportZone.Application.DTOs.Usuario;
using SportZone.Application.Interfaces.Servicios;

namespace SportZone.API.Controllers;

[Route("api/[controller]")]
public class UsuariosController : BaseController
{
    private readonly IUsuarioServicio _servicio;

    public UsuariosController(IUsuarioServicio servicio)
    {
        _servicio = servicio;
    }

    // GET /api/Usuarios?page=1&pageSize=10 -> cada item incluye RolNombre (relación 1:N resuelta)
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PaginacionQueryDto query)
    {
        var resultado = await _servicio.GetAllAsync(query);
        return RespuestaOk(resultado);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var usuario = await _servicio.GetByIdAsync(id);
        return usuario == null ? NotFound() : RespuestaOk(usuario);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUsuarioDto dto)
    {
        var creado = await _servicio.CreateAsync(dto);
        return RespuestaCreado(creado, "Usuario creado");
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateUsuarioDto dto)
    {
        var actualizado = await _servicio.UpdateAsync(id, dto);
        return RespuestaOk(actualizado, "Usuario actualizado");
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _servicio.DeleteAsync(id);
        return RespuestaSinDatos("Usuario eliminado");
    }
}
