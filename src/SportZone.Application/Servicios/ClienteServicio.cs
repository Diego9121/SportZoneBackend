using SportZone.Application.Common.Exceptions;
using SportZone.Application.DTOs.Cliente;
using SportZone.Application.DTOs.Common;
using SportZone.Application.Interfaces;
using SportZone.Application.Interfaces.Servicios;

namespace SportZone.Application.Servicios;

public class ClienteServicio : IClienteServicio
{
    private readonly IRepository<Cliente> _repository;

    public ClienteServicio(IRepository<Cliente> repository)
    {
        _repository = repository;
    }

    public async Task<PagedResultDto<ClienteDto>> GetAllAsync(PaginacionQueryDto query)
    {
        var (items, totalCount) = await _repository.GetPagedAsync(query);

        return new PagedResultDto<ClienteDto>
        {
            Items = items.Select(MapToDto),
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<ClienteDto?> GetByIdAsync(int id)
    {
        var cliente = await _repository.GetByIdAsync(id);
        return cliente == null ? null : MapToDto(cliente);
    }

    public async Task<ClienteDto> CreateAsync(CreateClienteDto dto)
    {
        if (!string.IsNullOrEmpty(dto.Documento) &&
            await _repository.ExisteAsync(c => c.Documento == dto.Documento))
            throw new ValidationException($"Ya existe un cliente con el documento '{dto.Documento}'");

        var cliente = new Cliente
        {
            TipoDocumento = dto.TipoDocumento,
            Documento = dto.Documento,
            Nombre = dto.Nombre,
            Telefono = dto.Telefono,
            Email = dto.Email,
            Direccion = dto.Direccion
        };

        var creado = await _repository.CreateAsync(cliente);
        return MapToDto(creado);
    }

    public async Task<ClienteDto> UpdateAsync(int id, UpdateClienteDto dto)
    {
        var cliente = await _repository.GetByIdAsync(id);
        if (cliente == null)
            throw new NotFoundException($"Cliente con Id {id} no encontrado");

        if (!string.IsNullOrEmpty(dto.Documento) &&
            await _repository.ExisteAsync(c => c.Documento == dto.Documento && c.Id != id))
            throw new ValidationException($"Ya existe otro cliente con el documento '{dto.Documento}'");

        cliente.TipoDocumento = dto.TipoDocumento;
        cliente.Documento = dto.Documento;
        cliente.Nombre = dto.Nombre;
        cliente.Telefono = dto.Telefono;
        cliente.Email = dto.Email;
        cliente.Direccion = dto.Direccion;

        var actualizado = await _repository.UpdateAsync(cliente);
        return MapToDto(actualizado);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }

    private static ClienteDto MapToDto(Cliente cliente)
    {
        return new ClienteDto
        {
            Id = cliente.Id,
            TipoDocumento = cliente.TipoDocumento,
            Documento = cliente.Documento,
            Nombre = cliente.Nombre,
            Telefono = cliente.Telefono,
            Email = cliente.Email,
            Direccion = cliente.Direccion,
            CreatedAt = cliente.CreatedAt
        };
    }
}
