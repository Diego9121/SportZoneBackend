namespace SportZone.Infrastructure.Repositories;

public class MovimientoStockRepository : Repository<MovimientoStock>, IMovimientoStockRepository
{
    public MovimientoStockRepository(ApplicationDbContext context, ICurrentUserService currentUser)
        : base(context, currentUser)
    {
    }

    public async Task<MovimientoStock?> GetByIdWithDetalleAsync(int id)
    {
        return await _context.MovimientosStock
            .Include(m => m.ArticuloVariante).ThenInclude(v => v.Articulo)
            .FirstOrDefaultAsync(m => m.Id == id);
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

        var totalCount = await query.CountAsync();
        var items = await query.Skip((pquery.Page - 1) * pquery.PageSize).Take(pquery.PageSize).ToListAsync();
        return (items, totalCount);
    }
}
