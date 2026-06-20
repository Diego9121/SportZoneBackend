using SportZone.Application.DTOs.ArticuloVariante;
using SportZone.Application.DTOs.Common;
using SportZone.Application.Interfaces.Servicios;

namespace SportZone.API.Controllers;

[Route("api/[controller]")]
public class ArticuloVariantesController : BaseController
{
    private readonly IArticuloVarianteServicio _servicio;

    public ArticuloVariantesController(IArticuloVarianteServicio servicio)
    {
        _servicio = servicio;
    }

    // GET /api/ArticuloVariantes?page=1&pageSize=10&articuloId=3  (articuloId es opcional)
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PaginacionQueryDto query, [FromQuery] int? articuloId)
    {
        var resultado = await _servicio.GetAllAsync(query, articuloId);
        return RespuestaOk(resultado);
    }

    // Ruta de texto fija; {id:int} con restricción de tipo evita que choque con esta ruta
    [HttpGet("stock-bajo")]
    public async Task<IActionResult> GetStockBajo()
    {
        var resultado = await _servicio.GetStockBajoAsync();
        return RespuestaOk(resultado);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var variante = await _servicio.GetByIdAsync(id);
        return variante == null ? NotFound() : RespuestaOk(variante);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateArticuloVarianteDto dto)
    {
        var creada = await _servicio.CreateAsync(dto);
        return RespuestaCreado(creada, "Variante creada");
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateArticuloVarianteDto dto)
    {
        var actualizada = await _servicio.UpdateAsync(id, dto);
        return RespuestaOk(actualizada, "Variante actualizada");
    }

    [HttpPut("{id:int}/ajustar-stock")]
    public async Task<IActionResult> AjustarStock(int id, AjustarStockDto dto)
    {
        var actualizada = await _servicio.AjustarStockAsync(id, dto);
        return RespuestaOk(actualizada, "Stock ajustado");
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _servicio.DeleteAsync(id);
        return RespuestaSinDatos("Variante eliminada");
    }
}
