using SportZone.Application.Common.Exceptions;
using SportZone.Application.DTOs.Articulo;
using SportZone.Application.DTOs.Common;
using SportZone.Application.Interfaces;
using SportZone.Application.Interfaces.Servicios;

namespace SportZone.Application.Servicios;

public class ArticuloServicio : IArticuloServicio
{
    private readonly IArticuloRepository _repository;
    private readonly IRepository<Categoria> _categoriaRepository;
    private readonly IRepository<Marca> _marcaRepository;

    public ArticuloServicio(
        IArticuloRepository repository,
        IRepository<Categoria> categoriaRepository,
        IRepository<Marca> marcaRepository)
    {
        _repository = repository;
        _categoriaRepository = categoriaRepository;
        _marcaRepository = marcaRepository;
    }

    public async Task<PagedResultDto<ArticuloDto>> GetAllAsync(PaginacionQueryDto query)
    {
        var (items, totalCount) = await _repository.GetPagedWithDetallesAsync(query);

        return new PagedResultDto<ArticuloDto>
        {
            Items = items.Select(MapToDto),
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<ArticuloDto?> GetByIdAsync(int id)
    {
        var articulo = await _repository.GetByIdWithDetallesAsync(id);
        return articulo == null ? null : MapToDto(articulo);
    }

    public async Task<ArticuloDto> CreateAsync(CreateArticuloDto dto)
    {
        if (await _repository.ExisteAsync(a => a.Codigo == dto.Codigo))
            throw new ValidationException($"Ya existe un articulo con el codigo '{dto.Codigo}'");

        if (!await _categoriaRepository.ExisteAsync(c => c.Id == dto.CategoriaId))
            throw new ValidationException($"La categoria con Id {dto.CategoriaId} no existe");

        if (!await _marcaRepository.ExisteAsync(m => m.Id == dto.MarcaId))
            throw new ValidationException($"La marca con Id {dto.MarcaId} no existe");

        var articulo = new Articulo
        {
            CategoriaId = dto.CategoriaId,
            MarcaId = dto.MarcaId,
            Codigo = dto.Codigo,
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            Imagen = dto.Imagen,
            PrecioVenta = dto.PrecioVenta,
            PrecioCosto = dto.PrecioCosto
        };

        var creado = await _repository.CreateAsync(articulo);

        var creadoConDetalles = await _repository.GetByIdWithDetallesAsync(creado.Id);
        return MapToDto(creadoConDetalles!);
    }

    public async Task<ArticuloDto> UpdateAsync(int id, UpdateArticuloDto dto)
    {
        var articulo = await _repository.GetByIdAsync(id);
        if (articulo == null)
            throw new NotFoundException($"Articulo con Id {id} no encontrado");

        if (await _repository.ExisteAsync(a => a.Codigo == dto.Codigo && a.Id != id))
            throw new ValidationException($"Ya existe otro articulo con el codigo '{dto.Codigo}'");

        if (!await _categoriaRepository.ExisteAsync(c => c.Id == dto.CategoriaId))
            throw new ValidationException($"La categoria con Id {dto.CategoriaId} no existe");

        if (!await _marcaRepository.ExisteAsync(m => m.Id == dto.MarcaId))
            throw new ValidationException($"La marca con Id {dto.MarcaId} no existe");

        articulo.CategoriaId = dto.CategoriaId;
        articulo.MarcaId = dto.MarcaId;
        articulo.Codigo = dto.Codigo;
        articulo.Nombre = dto.Nombre;
        articulo.Descripcion = dto.Descripcion;
        articulo.Imagen = dto.Imagen;
        articulo.PrecioVenta = dto.PrecioVenta;
        articulo.PrecioCosto = dto.PrecioCosto;

        await _repository.UpdateAsync(articulo);

        var actualizadoConDetalles = await _repository.GetByIdWithDetallesAsync(id);
        return MapToDto(actualizadoConDetalles!);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }

    private static ArticuloDto MapToDto(Articulo articulo)
    {
        return new ArticuloDto
        {
            Id = articulo.Id,
            CategoriaId = articulo.CategoriaId,
            CategoriaNombre = articulo.Categoria?.Nombre ?? string.Empty,
            MarcaId = articulo.MarcaId,
            MarcaNombre = articulo.Marca?.Nombre ?? string.Empty,
            Codigo = articulo.Codigo,
            Nombre = articulo.Nombre,
            Descripcion = articulo.Descripcion,
            Imagen = articulo.Imagen,
            PrecioVenta = articulo.PrecioVenta,
            PrecioCosto = articulo.PrecioCosto,
            TotalVariantes = articulo.Variantes?.Count ?? 0,
            StockTotal = articulo.Variantes?.Sum(v => v.Stock) ?? 0,
            CreatedAt = articulo.CreatedAt
        };
    }
}
