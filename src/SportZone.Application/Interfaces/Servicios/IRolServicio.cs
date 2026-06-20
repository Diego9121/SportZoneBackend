using SportZone.Application.DTOs.Common;
using SportZone.Application.DTOs.Rol;

namespace SportZone.Application.Interfaces.Servicios;

public interface IRolServicio
{
    Task<PagedResultDto<RolDto>> GetAllAsync(PaginacionQueryDto query);
    Task<RolDto?> GetByIdAsync(int id);
    Task<RolDto> CreateAsync(CreateRolDto dto);
    Task<RolDto> UpdateAsync(int id, UpdateRolDto dto);
    Task DeleteAsync(int id);
}
