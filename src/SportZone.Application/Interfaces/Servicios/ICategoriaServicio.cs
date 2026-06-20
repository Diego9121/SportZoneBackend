using SportZone.Application.DTOs.Categoria;
using SportZone.Application.DTOs.Common;

namespace SportZone.Application.Interfaces.Servicios;

public interface ICategoriaServicio
{
    Task<PagedResultDto<CategoriaDto>> GetAllAsync(PaginacionQueryDto query);
    Task<CategoriaDto?> GetByIdAsync(int id);
    Task<CategoriaDto> CreateAsync(CreateCategoriaDto dto);
    Task<CategoriaDto> UpdateAsync(int id, UpdateCategoriaDto dto);
    Task DeleteAsync(int id);
}
