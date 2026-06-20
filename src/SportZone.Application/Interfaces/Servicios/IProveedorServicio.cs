using SportZone.Application.DTOs.Common;
using SportZone.Application.DTOs.Proveedor;

namespace SportZone.Application.Interfaces.Servicios;

public interface IProveedorServicio
{
    Task<PagedResultDto<ProveedorDto>> GetAllAsync(PaginacionQueryDto query);
    Task<ProveedorDto?> GetByIdAsync(int id);
    Task<ProveedorDto> CreateAsync(CreateProveedorDto dto);
    Task<ProveedorDto> UpdateAsync(int id, UpdateProveedorDto dto);
    Task DeleteAsync(int id);
}
