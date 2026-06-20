using SportZone.Application.DTOs.Common;
using SportZone.Application.DTOs.Venta;

namespace SportZone.Application.Interfaces.Servicios;

public interface IVentaServicio
{
    Task<PagedResultDto<VentaDto>> GetAllAsync(PaginacionQueryDto query);
    Task<VentaDto?> GetByIdAsync(int id);
    Task<VentaDto> CreateAsync(CreateVentaDto dto);
    Task<VentaDto> AnularAsync(int id);
}
