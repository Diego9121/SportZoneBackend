using SportZone.Application.DTOs.Articulo;
using SportZone.Application.DTOs.Common;

namespace SportZone.Application.Interfaces.Servicios;

public interface IArticuloServicio
{
    Task<PagedResultDto<ArticuloDto>> GetAllAsync(PaginacionQueryDto query);
    Task<ArticuloDto?> GetByIdAsync(int id);
    Task<ArticuloDto> CreateAsync(CreateArticuloDto dto);
    Task<ArticuloDto> UpdateAsync(int id, UpdateArticuloDto dto);
    Task DeleteAsync(int id);
}
