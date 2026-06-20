using SportZone.Application.Common.Exceptions;
using SportZone.Application.DTOs.Common;
using SportZone.Application.DTOs.Marca;
using SportZone.Application.Interfaces;
using SportZone.Application.Interfaces.Servicios;

namespace SportZone.Application.Servicios;

public class MarcaServicio : IMarcaServicio
{
    private readonly IRepository<Marca> _repository;

    public MarcaServicio(IRepository<Marca> repository)
    {
        _repository = repository;
    }

    public async Task<PagedResultDto<MarcaDto>> GetAllAsync(PaginacionQueryDto query)
    {
        var (items, totalCount) = await _repository.GetPagedAsync(query);

        return new PagedResultDto<MarcaDto>
        {
            Items = items.Select(MapToDto),
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<MarcaDto?> GetByIdAsync(int id)
    {
        var marca = await _repository.GetByIdAsync(id);
        return marca == null ? null : MapToDto(marca);
    }

    // Logo llega como URL: el frontend la sube primero a /api/Imagenes y manda esa URL aquí
    public async Task<MarcaDto> CreateAsync(CreateMarcaDto dto)
    {
        if (await _repository.ExisteAsync(m => m.Nombre == dto.Nombre))
            throw new ValidationException($"Ya existe una marca con el nombre '{dto.Nombre}'");

        var marca = new Marca
        {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            Logo = dto.Logo
        };

        var creada = await _repository.CreateAsync(marca);
        return MapToDto(creada);
    }

    public async Task<MarcaDto> UpdateAsync(int id, UpdateMarcaDto dto)
    {
        var marca = await _repository.GetByIdAsync(id);
        if (marca == null)
            throw new NotFoundException($"Marca con Id {id} no encontrada");

        if (await _repository.ExisteAsync(m => m.Nombre == dto.Nombre && m.Id != id))
            throw new ValidationException($"Ya existe otra marca con el nombre '{dto.Nombre}'");

        marca.Nombre = dto.Nombre;
        marca.Descripcion = dto.Descripcion;
        marca.Logo = dto.Logo;

        var actualizada = await _repository.UpdateAsync(marca);
        return MapToDto(actualizada);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }

    private static MarcaDto MapToDto(Marca marca)
    {
        return new MarcaDto
        {
            Id = marca.Id,
            Nombre = marca.Nombre,
            Descripcion = marca.Descripcion,
            CreatedAt = marca.CreatedAt,
            Logo = marca.Logo
        };
    }
}
