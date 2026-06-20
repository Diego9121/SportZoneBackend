using SportZone.Application.DTOs.Common;
using SportZone.Application.DTOs.Ingreso;

namespace SportZone.Application.Interfaces.Servicios;

public interface IIngresoServicio
{
    Task<PagedResultDto<IngresoDto>> GetAllAsync(PaginacionQueryDto query);
    Task<IngresoDto?> GetByIdAsync(int id);
    Task<IngresoDto> CreateAsync(CreateIngresoDto dto);
}
