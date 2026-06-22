using SportZone.Application.DTOs.Common;
using SportZone.Application.DTOs.MovimientoStock;
using SportZone.Application.Interfaces;
using SportZone.Application.Interfaces.Servicios;

namespace SportZone.Application.Servicios;

public class MovimientoStockServicio : IMovimientoStockServicio
{
    private readonly IMovimientoStockRepository _repository;

    public MovimientoStockServicio(IMovimientoStockRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResultDto<MovimientoStockDto>> GetAllAsync(PaginacionQueryDto query, int? articuloVarianteId)
    {
        var (items, totalCount) = await _repository.GetPagedWithDetalleAsync(query, articuloVarianteId);

        return new PagedResultDto<MovimientoStockDto>
        {
            Items = items.Select(MapToDto),
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<IEnumerable<MovimientoStockDto>> GetByVarianteIdAsync(int varianteId)
    {
        var movimientos = await _repository.GetByVarianteIdAsync(varianteId);
        return movimientos.Select(MapToDto);
    }

    private static MovimientoStockDto MapToDto(MovimientoStock movimiento)
    {
        return new MovimientoStockDto
        {
            Id = movimiento.Id,
            ArticuloVarianteId = movimiento.ArticuloVarianteId,
            ArticuloVarianteDescripcion = DescribirVariante(movimiento.ArticuloVariante),
            IngresoId = movimiento.IngresoId,
            VentaId = movimiento.VentaId,
            TipoMovimiento = movimiento.TipoMovimiento,
            Cantidad = movimiento.Cantidad,
            NumeroDoc = movimiento.NumeroDoc,
            CreatedAt = movimiento.CreatedAt
        };
    }

    private static string DescribirVariante(ArticuloVariante? variante)
    {
        if (variante == null) return string.Empty;
        var partes = new List<string>();
        if (variante.Articulo != null) partes.Add(variante.Articulo.Nombre);
        if (!string.IsNullOrEmpty(variante.TallaUs)) partes.Add($"Talla {variante.TallaUs}");
        if (!string.IsNullOrEmpty(variante.Color)) partes.Add(variante.Color);
        return string.Join(" - ", partes);
    }
}
