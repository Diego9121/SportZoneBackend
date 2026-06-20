using SportZone.Application.DTOs.Common;
using SportZone.Application.DTOs.Proveedor;
using SportZone.Application.Interfaces.Servicios;

namespace SportZone.API.Controllers;

[Route("api/[controller]")]
public class ProveedoresController : BaseController
{
    private readonly IProveedorServicio _servicio;

    public ProveedoresController(IProveedorServicio servicio)
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
        var proveedor = await _servicio.GetByIdAsync(id);
        return proveedor == null ? NotFound() : RespuestaOk(proveedor);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProveedorDto dto)
    {
        var creado = await _servicio.CreateAsync(dto);
        return RespuestaCreado(creado, "Proveedor creado");
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateProveedorDto dto)
    {
        var actualizado = await _servicio.UpdateAsync(id, dto);
        return RespuestaOk(actualizado, "Proveedor actualizado");
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _servicio.DeleteAsync(id);
        return RespuestaSinDatos("Proveedor eliminado");
    }
}
