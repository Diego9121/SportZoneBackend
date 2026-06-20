namespace SportZone.Infrastructure.Repositories;

public class ArticuloRepository : Repository<Articulo>, IArticuloRepository
{
    public ArticuloRepository(ApplicationDbContext context, ICurrentUserService currentUser)
        : base(context, currentUser)
    {
    }

    public async Task<Articulo?> GetByIdWithDetallesAsync(int id)
    {
        return await _context.Articulos
            .Include(a => a.Categoria)
            .Include(a => a.Marca)
            .Include(a => a.Variantes)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<(IEnumerable<Articulo> Items, int TotalCount)> GetPagedWithDetallesAsync(PaginacionQueryDto pquery)
    {
        var query = AplicarFiltroYOrden(
            _context.Articulos
                .Include(a => a.Categoria)
                .Include(a => a.Marca)
                .Include(a => a.Variantes)
                .OrderBy(a => a.Id),
            pquery);

        var totalCount = await query.CountAsync();
        var items = await query.Skip((pquery.Page - 1) * pquery.PageSize).Take(pquery.PageSize).ToListAsync();
        return (items, totalCount);
    }
}
