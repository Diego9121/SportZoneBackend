using SportZone.Application.Common.Exceptions;
using SportZone.Application.DTOs.Common;
using SportZone.Application.DTOs.Rol;
using SportZone.Application.Interfaces;
using SportZone.Application.Interfaces.Servicios;

namespace SportZone.Application.Servicios;

public class RolServicio : IRolServicio
{
    private readonly IRepository<Rol> _repository;

    public RolServicio(IRepository<Rol> repository)
    {
        _repository = repository;
    }

    // Lista paginada: el Repository calcula Skip/Take/Count, aquí solo se arma el DTO de respuesta
    public async Task<PagedResultDto<RolDto>> GetAllAsync(PaginacionQueryDto query)
    {
        var (items, totalCount) = await _repository.GetPagedAsync(query);

        return new PagedResultDto<RolDto>
        {
            Items = items.Select(MapToDto),
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<RolDto?> GetByIdAsync(int id)
    {
        var rol = await _repository.GetByIdAsync(id);
        return rol == null ? null : MapToDto(rol);
    }

    // CreateById ya no se asigna aquí: el Repository lo hace automáticamente vía ICurrentUserService
    public async Task<RolDto> CreateAsync(CreateRolDto dto)
    {
        if (await _repository.ExisteAsync(r => r.Nombre == dto.Nombre))
            throw new ValidationException($"Ya existe un rol con el nombre '{dto.Nombre}'");

        var rol = new Rol
        {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion
        };

        var creado = await _repository.CreateAsync(rol);
        return MapToDto(creado);
    }

    public async Task<RolDto> UpdateAsync(int id, UpdateRolDto dto)
    {
        var rol = await _repository.GetByIdAsync(id);
        if (rol == null)
            throw new NotFoundException($"Rol con Id {id} no encontrado");

        if (await _repository.ExisteAsync(r => r.Nombre == dto.Nombre && r.Id != id))
            throw new ValidationException($"Ya existe otro rol con el nombre '{dto.Nombre}'");

        rol.Nombre = dto.Nombre;
        rol.Descripcion = dto.Descripcion;

        var actualizado = await _repository.UpdateAsync(rol);
        return MapToDto(actualizado);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }

    private static RolDto MapToDto(Rol rol)
    {
        return new RolDto
        {
            Id = rol.Id,
            Nombre = rol.Nombre,
            Descripcion = rol.Descripcion,
            CreatedAt = rol.CreatedAt
        };
    }
}
