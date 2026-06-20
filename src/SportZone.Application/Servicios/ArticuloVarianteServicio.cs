using SportZone.Application.Common.Exceptions;
using SportZone.Application.DTOs.ArticuloVariante;
using SportZone.Application.DTOs.Common;
using SportZone.Application.Interfaces;
using SportZone.Application.Interfaces.Servicios;

namespace SportZone.Application.Servicios;

public class ArticuloVarianteServicio : IArticuloVarianteServicio
{
    private readonly IArticuloVarianteRepository _repository;
    private readonly IArticuloRepository _articuloRepository;

    public ArticuloVarianteServicio(IArticuloVarianteRepository repository, IArticuloRepository articuloRepository)
    {
        _repository = repository;
        _articuloRepository = articuloRepository;
    }

    public async Task<PagedResultDto<ArticuloVarianteDto>> GetAllAsync(PaginacionQueryDto query, int? articuloId)
    {
        var (items, totalCount) = await _repository.GetPagedWithArticuloAsync(query, articuloId);

        return new PagedResultDto<ArticuloVarianteDto>
        {
            Items = items.Select(MapToDto),
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<ArticuloVarianteDto?> GetByIdAsync(int id)
    {
        var variante = await _repository.GetByIdWithArticuloAsync(id);
        return variante == null ? null : MapToDto(variante);
    }

    public async Task<IEnumerable<ArticuloVarianteDto>> GetStockBajoAsync()
    {
        var variantes = await _repository.GetStockBajoAsync();
        return variantes.Select(MapToDto);
    }

    public async Task<ArticuloVarianteDto> CreateAsync(CreateArticuloVarianteDto dto)
    {
        if (!await _articuloRepository.ExisteAsync(a => a.Id == dto.ArticuloId))
            throw new ValidationException($"El articulo con Id {dto.ArticuloId} no existe");

        if (await _repository.ExisteAsync(v =>
                v.ArticuloId == dto.ArticuloId && v.TallaUs == dto.TallaUs && v.Color == dto.Color))
            throw new ValidationException("Ya existe una variante con esa talla y color para este articulo");

        if (!string.IsNullOrEmpty(dto.CodigoBarras) &&
            await _repository.ExisteAsync(v => v.CodigoBarras == dto.CodigoBarras))
            throw new ValidationException($"Ya existe una variante con el codigo de barras '{dto.CodigoBarras}'");

        var variante = new ArticuloVariante
        {
            ArticuloId = dto.ArticuloId,
            TallaUs = dto.TallaUs,
            TallaEu = dto.TallaEu,
            TallaUk = dto.TallaUk,
            TallaCm = dto.TallaCm,
            Color = dto.Color,
            CodigoBarras = dto.CodigoBarras,
            Stock = dto.Stock,
            StockMinimo = dto.StockMinimo,
            PrecioVenta = dto.PrecioVenta,
            PrecioCosto = dto.PrecioCosto
        };

        var creada = await _repository.CreateAsync(variante);

        var creadaConArticulo = await _repository.GetByIdWithArticuloAsync(creada.Id);
        return MapToDto(creadaConArticulo!);
    }

    public async Task<ArticuloVarianteDto> UpdateAsync(int id, UpdateArticuloVarianteDto dto)
    {
        var variante = await _repository.GetByIdAsync(id);
        if (variante == null)
            throw new NotFoundException($"Variante con Id {id} no encontrada");

        if (await _repository.ExisteAsync(v =>
                v.ArticuloId == variante.ArticuloId && v.TallaUs == dto.TallaUs && v.Color == dto.Color && v.Id != id))
            throw new ValidationException("Ya existe otra variante con esa talla y color para este articulo");

        if (!string.IsNullOrEmpty(dto.CodigoBarras) &&
            await _repository.ExisteAsync(v => v.CodigoBarras == dto.CodigoBarras && v.Id != id))
            throw new ValidationException($"Ya existe otra variante con el codigo de barras '{dto.CodigoBarras}'");

        variante.TallaUs = dto.TallaUs;
        variante.TallaEu = dto.TallaEu;
        variante.TallaUk = dto.TallaUk;
        variante.TallaCm = dto.TallaCm;
        variante.Color = dto.Color;
        variante.CodigoBarras = dto.CodigoBarras;
        variante.StockMinimo = dto.StockMinimo;
        variante.PrecioVenta = dto.PrecioVenta;
        variante.PrecioCosto = dto.PrecioCosto;

        await _repository.UpdateAsync(variante);

        var actualizadaConArticulo = await _repository.GetByIdWithArticuloAsync(id);
        return MapToDto(actualizadaConArticulo!);
    }

    // Único lugar fuera de Ingreso/Venta donde se permite tocar Stock directamente: corrección manual
    public async Task<ArticuloVarianteDto> AjustarStockAsync(int id, AjustarStockDto dto)
    {
        var variante = await _repository.GetByIdAsync(id);
        if (variante == null)
            throw new NotFoundException($"Variante con Id {id} no encontrada");

        var nuevoStock = variante.Stock + dto.Cantidad;
        if (nuevoStock < 0)
            throw new ValidationException($"El ajuste dejaria el stock en {nuevoStock}, no puede ser negativo");

        variante.Stock = nuevoStock;
        await _repository.UpdateAsync(variante);

        var actualizadaConArticulo = await _repository.GetByIdWithArticuloAsync(id);
        return MapToDto(actualizadaConArticulo!);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }

    private static ArticuloVarianteDto MapToDto(ArticuloVariante variante)
    {
        return new ArticuloVarianteDto
        {
            Id = variante.Id,
            ArticuloId = variante.ArticuloId,
            ArticuloNombre = variante.Articulo?.Nombre ?? string.Empty,
            ArticuloCodigo = variante.Articulo?.Codigo ?? string.Empty,
            TallaUs = variante.TallaUs,
            TallaEu = variante.TallaEu,
            TallaUk = variante.TallaUk,
            TallaCm = variante.TallaCm,
            Color = variante.Color,
            CodigoBarras = variante.CodigoBarras,
            Stock = variante.Stock,
            StockMinimo = variante.StockMinimo,
            StockBajo = variante.Stock <= variante.StockMinimo,
            PrecioVenta = variante.PrecioVenta,
            PrecioCosto = variante.PrecioCosto,
            CreatedAt = variante.CreatedAt
        };
    }
}
