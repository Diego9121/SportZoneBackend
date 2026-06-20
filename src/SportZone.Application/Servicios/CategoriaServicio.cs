using SportZone.Application.DTOs.Categoria;
using SportZone.Application.DTOs.Common;
using SportZone.Application.Interfaces;
using SportZone.Application.Interfaces.Servicios;
using SportZone.Application.Common.Exceptions;
using SportZone.Domain.Entities;

namespace SportZone.Application.Servicios;

public class CategoriaServicio : ICategoriaServicio
{
    private readonly IRepository<Categoria> _repository;

    public CategoriaServicio(IRepository<Categoria> repository)
    {
        _repository = repository;
    }

    public async Task<PagedResultDto<CategoriaDto>> GetAllAsync(PaginacionQueryDto query)
    {
        var (items, totalCount) = await _repository.GetPagedAsync(query);

        return new PagedResultDto<CategoriaDto>
        {
            Items = items.Select(MapToDto),
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<CategoriaDto?> GetByIdAsync(int id)
    {
        var categoria = await _repository.GetByIdAsync(id);
        return categoria == null ? null : MapToDto(categoria);
    }

    public async Task<CategoriaDto> CreateAsync(CreateCategoriaDto dto)
    {
        if (await _repository.ExisteAsync(c => c.Nombre == dto.Nombre))
            throw new ValidationException($"Ya existe una categoria con el nombre '{dto.Nombre}'");

        var categoria = new Categoria
        {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion
        };

        var creada = await _repository.CreateAsync(categoria);
        return MapToDto(creada);
    }

    public async Task<CategoriaDto> UpdateAsync(int id, UpdateCategoriaDto dto)
    {
        var categoria = await _repository.GetByIdAsync(id);
        if (categoria == null)
            throw new NotFoundException($"Categoria con Id {id} no encontrada");

        // Excluye el propio Id: si no, la categoría se "chocaría" consigo misma al guardar con el mismo nombre
        if (await _repository.ExisteAsync(c => c.Nombre == dto.Nombre && c.Id != id))
            throw new ValidationException($"Ya existe otra categoria con el nombre '{dto.Nombre}'");

        categoria.Nombre = dto.Nombre;
        categoria.Descripcion = dto.Descripcion;

        var actualizada = await _repository.UpdateAsync(categoria);
        return MapToDto(actualizada);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }

    private static CategoriaDto MapToDto(Categoria categoria)
    {
        return new CategoriaDto
        {
            Id = categoria.Id,
            Nombre = categoria.Nombre,
            Descripcion = categoria.Descripcion,
            CreatedAt = categoria.CreatedAt
        };
    }
}
