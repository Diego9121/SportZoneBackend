using SportZone.Application.DTOs.ArticuloVariante;
using SportZone.Application.DTOs.Common;

namespace SportZone.Application.Interfaces.Servicios;

public interface IArticuloVarianteServicio
{
    Task<PagedResultDto<ArticuloVarianteDto>> GetAllAsync(PaginacionQueryDto query, int? articuloId);
    Task<ArticuloVarianteDto?> GetByIdAsync(int id);
    Task<IEnumerable<ArticuloVarianteDto>> GetStockBajoAsync();
    Task<ArticuloVarianteDto> CreateAsync(CreateArticuloVarianteDto dto);
    Task<ArticuloVarianteDto> UpdateAsync(int id, UpdateArticuloVarianteDto dto);
    Task<ArticuloVarianteDto> AjustarStockAsync(int id, AjustarStockDto dto);
    Task DeleteAsync(int id);
}
