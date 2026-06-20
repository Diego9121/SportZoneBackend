using SportZone.Application.Common.Exceptions;
using SportZone.Application.DTOs.Common;
using SportZone.Application.DTOs.Proveedor;
using SportZone.Application.Interfaces;
using SportZone.Application.Interfaces.Servicios;

namespace SportZone.Application.Servicios;

// Ya no maneja relación con Marca (esa tabla se eliminó); usa el repositorio genérico directamente.
public class ProveedorServicio : IProveedorServicio
{
    private readonly IRepository<Proveedor> _repository;

    public ProveedorServicio(IRepository<Proveedor> repository)
    {
        _repository = repository;
    }

    public async Task<PagedResultDto<ProveedorDto>> GetAllAsync(PaginacionQueryDto query)
    {
        var (items, totalCount) = await _repository.GetPagedAsync(query);

        return new PagedResultDto<ProveedorDto>
        {
            Items = items.Select(MapToDto),
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<ProveedorDto?> GetByIdAsync(int id)
    {
        var proveedor = await _repository.GetByIdAsync(id);
        return proveedor == null ? null : MapToDto(proveedor);
    }

    public async Task<ProveedorDto> CreateAsync(CreateProveedorDto dto)
    {
        if (await _repository.ExisteAsync(p => p.Nombre == dto.Nombre))
            throw new ValidationException($"Ya existe un proveedor con el nombre '{dto.Nombre}'");

        var proveedor = new Proveedor
        {
            Nombre = dto.Nombre,
            Contacto = dto.Contacto,
            Telefono = dto.Telefono,
            Email = dto.Email,
            Direccion = dto.Direccion
        };

        var creado = await _repository.CreateAsync(proveedor);
        return MapToDto(creado);
    }

    public async Task<ProveedorDto> UpdateAsync(int id, UpdateProveedorDto dto)
    {
        var proveedor = await _repository.GetByIdAsync(id);
        if (proveedor == null)
            throw new NotFoundException($"Proveedor con Id {id} no encontrado");

        if (await _repository.ExisteAsync(p => p.Nombre == dto.Nombre && p.Id != id))
            throw new ValidationException($"Ya existe otro proveedor con el nombre '{dto.Nombre}'");

        proveedor.Nombre = dto.Nombre;
        proveedor.Contacto = dto.Contacto;
        proveedor.Telefono = dto.Telefono;
        proveedor.Email = dto.Email;
        proveedor.Direccion = dto.Direccion;

        var actualizado = await _repository.UpdateAsync(proveedor);
        return MapToDto(actualizado);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }

    private static ProveedorDto MapToDto(Proveedor proveedor)
    {
        return new ProveedorDto
        {
            Id = proveedor.Id,
            Nombre = proveedor.Nombre,
            Contacto = proveedor.Contacto,
            Telefono = proveedor.Telefono,
            Email = proveedor.Email,
            Direccion = proveedor.Direccion,
            CreatedAt = proveedor.CreatedAt
        };
    }
}
