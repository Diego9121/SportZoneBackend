using SportZone.Application.DTOs.Common;

namespace SportZone.Application.Interfaces;

public interface IMovimientoStockRepository : IRepository<MovimientoStock>
{
    Task<MovimientoStock?> GetByIdWithDetalleAsync(int id);

    // articuloVarianteId opcional: permite ver "todo el historial de esta variante" desde el mismo endpoint
    Task<(IEnumerable<MovimientoStock> Items, int TotalCount)> GetPagedWithDetalleAsync(
        PaginacionQueryDto query, int? articuloVarianteId);
}
