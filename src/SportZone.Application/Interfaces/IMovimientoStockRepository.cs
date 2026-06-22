using SportZone.Application.DTOs.Common;

namespace SportZone.Application.Interfaces;

public interface IMovimientoStockRepository : IRepository<MovimientoStock>
{
    // Devuelve TODO el historial de movimientos de una variante (no busca por el Id propio del movimiento,
    // que no tiene utilidad real para el usuario: nadie consulta "el movimiento Id 7", consulta "el historial
    // de esta variante").
    Task<IEnumerable<MovimientoStock>> GetByVarianteIdAsync(int varianteId);

    // articuloVarianteId opcional: permite ver "todo el historial de esta variante" desde el mismo endpoint
    Task<(IEnumerable<MovimientoStock> Items, int TotalCount)> GetPagedWithDetalleAsync(
        PaginacionQueryDto query, int? articuloVarianteId);
}
