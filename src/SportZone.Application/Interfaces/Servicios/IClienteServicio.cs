using SportZone.Application.DTOs.Cliente;
using SportZone.Application.DTOs.Common;

namespace SportZone.Application.Interfaces.Servicios;

public interface IClienteServicio
{
    Task<PagedResultDto<ClienteDto>> GetAllAsync(PaginacionQueryDto query);
    Task<ClienteDto?> GetByIdAsync(int id);
    Task<ClienteDto> CreateAsync(CreateClienteDto dto);
    Task<ClienteDto> UpdateAsync(int id, UpdateClienteDto dto);
    Task<ClienteDto> AjustarPuntosAsync(int id, AjustarPuntosDto dto);
    Task DeleteAsync(int id);
}
