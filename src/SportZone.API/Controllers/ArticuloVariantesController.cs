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

    // GET /api/ArticuloVariantes?page=1&pageSize=10&articuloId=3&talla=42&stock=5&filter=umbro
    // articuloId, talla y stock son opcionales. filter ya busca por nombre del articulo, color, codigo de
    // barras, tallas y precios. talla busca ese valor en TallaUs/TallaEu/TallaUk/TallaCm (los 4 formatos).
    // stock NO es igualdad: es "al menos este stock" (ej. stock=5 -> devuelve variantes con Stock >= 5).
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] PaginacionQueryDto query, [FromQuery] int? articuloId, [FromQuery] string? talla, [FromQuery] int? stock)
    {
        var resultado = await _servicio.GetAllAsync(query, articuloId, talla, stock);
        return RespuestaOk(resultado);
    }

    // GET /api/ArticuloVariantes/catalogo?... (mismos filtros que GetAll: articuloId, talla, stock, filter, etc.)
    // A diferencia de GetAll, cada item trae el nombre completo del articulo y la URL de su imagen
    // (pantallas de catálogo, POS o consulta móvil que necesitan mostrar la foto del producto).
    [HttpGet("catalogo")]
    public async Task<IActionResult> GetCatalogo(
        [FromQuery] PaginacionQueryDto query, [FromQuery] int? articuloId, [FromQuery] string? talla, [FromQuery] int? stock)
    {
        var resultado = await _servicio.GetCatalogoAsync(query, articuloId, talla, stock);
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

    // Ruta de texto fija; pensada para el escaneo de código de barras en el punto de venta
    [HttpGet("codigo-barras/{codigo}")]
    public async Task<IActionResult> GetByCodigoBarras(string codigo)
    {
        var variante = await _servicio.GetByCodigoBarrasAsync(codigo);
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
