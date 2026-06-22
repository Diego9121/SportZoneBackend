namespace SportZone.Infrastructure.Repositories;

public class MovimientoStockRepository : Repository<MovimientoStock>, IMovimientoStockRepository
{
    public MovimientoStockRepository(ApplicationDbContext context, ICurrentUserService currentUser)
        : base(context, currentUser)
    {
    }

    public async Task<IEnumerable<MovimientoStock>> GetByVarianteIdAsync(int varianteId)
    {
        return await _context.MovimientosStock
            .Include(m => m.ArticuloVariante).ThenInclude(v => v.Articulo)
            .Where(m => m.ArticuloVarianteId == varianteId)
            .OrderByDescending(m => m.Id)
            .ToListAsync();
    }

    public async Task<(IEnumerable<MovimientoStock> Items, int TotalCount)> GetPagedWithDetalleAsync(
        PaginacionQueryDto pquery, int? articuloVarianteId)
    {
        var query = _context.MovimientosStock
            .Include(m => m.ArticuloVariante).ThenInclude(v => v.Articulo)
            .AsQueryable();

        if (articuloVarianteId.HasValue)
            query = query.Where(m => m.ArticuloVarianteId == articuloVarianteId.Value);

        query = AplicarFiltroYOrden(query.OrderByDescending(m => m.Id), pquery);

        return await PaginarAsync(query, pquery);
    }
}
