using SportZone.Application.DTOs.Common;
using SportZone.Application.DTOs.Usuario;

namespace SportZone.Application.Interfaces.Servicios;

public interface IUsuarioServicio
{
    Task<PagedResultDto<UsuarioDto>> GetAllAsync(PaginacionQueryDto query);
    Task<UsuarioDto?> GetByIdAsync(int id);
    Task<UsuarioDto> CreateAsync(CreateUsuarioDto dto);
    Task<UsuarioDto> UpdateAsync(int id, UpdateUsuarioDto dto);
    Task DeleteAsync(int id);
}
