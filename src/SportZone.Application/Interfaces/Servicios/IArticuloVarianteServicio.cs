using SportZone.Application.DTOs.ArticuloVariante;
using SportZone.Application.DTOs.Common;

namespace SportZone.Application.Interfaces.Servicios;

public interface IArticuloVarianteServicio
{
    Task<PagedResultDto<ArticuloVarianteDto>> GetAllAsync(PaginacionQueryDto query, int? articuloId, string? talla, int? stock);

    // Igual que GetAllAsync pero con nombre completo del articulo + URL de imagen (catálogo/POS/consulta móvil)
    Task<PagedResultDto<ArticuloVarianteCatalogoDto>> GetCatalogoAsync(PaginacionQueryDto query, int? articuloId, string? talla, int? stock);
    Task<ArticuloVarianteDto?> GetByIdAsync(int id);
    Task<ArticuloVarianteDto?> GetByCodigoBarrasAsync(string codigoBarras);
    Task<IEnumerable<ArticuloVarianteDto>> GetStockBajoAsync();
    Task<ArticuloVarianteDto> CreateAsync(CreateArticuloVarianteDto dto);
    Task<ArticuloVarianteDto> UpdateAsync(int id, UpdateArticuloVarianteDto dto);
    Task<ArticuloVarianteDto> AjustarStockAsync(int id, AjustarStockDto dto);
    Task DeleteAsync(int id);
}
