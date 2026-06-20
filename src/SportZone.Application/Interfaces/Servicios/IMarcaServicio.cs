using SportZone.Application.DTOs.Common;
using SportZone.Application.DTOs.Marca;

namespace SportZone.Application.Interfaces.Servicios;

public interface IMarcaServicio
{
    Task<PagedResultDto<MarcaDto>> GetAllAsync(PaginacionQueryDto query);
    Task<MarcaDto?> GetByIdAsync(int id);
    Task<MarcaDto> CreateAsync(CreateMarcaDto dto);
    Task<MarcaDto> UpdateAsync(int id, UpdateMarcaDto dto);
    Task DeleteAsync(int id);
}
