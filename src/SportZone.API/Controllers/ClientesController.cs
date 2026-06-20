using SportZone.Application.DTOs.Cliente;
using SportZone.Application.DTOs.Common;
using SportZone.Application.Interfaces.Servicios;

namespace SportZone.API.Controllers;

[Route("api/[controller]")]
public class ClientesController : BaseController
{
    private readonly IClienteServicio _servicio;

    public ClientesController(IClienteServicio servicio)
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
        var cliente = await _servicio.GetByIdAsync(id);
        return cliente == null ? NotFound() : RespuestaOk(cliente);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateClienteDto dto)
    {
        var creado = await _servicio.CreateAsync(dto);
        return RespuestaCreado(creado, "Cliente creado");
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateClienteDto dto)
    {
        var actualizado = await _servicio.UpdateAsync(id, dto);
        return RespuestaOk(actualizado, "Cliente actualizado");
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _servicio.DeleteAsync(id);
        return RespuestaSinDatos("Cliente eliminado");
    }
}
