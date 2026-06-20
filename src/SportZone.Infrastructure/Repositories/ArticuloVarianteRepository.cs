namespace SportZone.Infrastructure.Repositories;

public class ArticuloVarianteRepository : Repository<ArticuloVariante>, IArticuloVarianteRepository
{
    public ArticuloVarianteRepository(ApplicationDbContext context, ICurrentUserService currentUser)
        : base(context, currentUser)
    {
    }

    public async Task<ArticuloVariante?> GetByIdWithArticuloAsync(int id)
    {
        return await _context.ArticuloVariantes.Include(v => v.Articulo).FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<ArticuloVariante?> GetByCodigoBarrasAsync(string codigoBarras)
    {
        return await _context.ArticuloVariantes
            .Include(v => v.Articulo)
            .FirstOrDefaultAsync(v => v.CodigoBarras == codigoBarras);
    }

    public async Task<(IEnumerable<ArticuloVariante> Items, int TotalCount)> GetPagedWithArticuloAsync(
        PaginacionQueryDto pquery, int? articuloId)
    {
        var query = _context.ArticuloVariantes.Include(v => v.Articulo).AsQueryable();

        if (articuloId.HasValue)
            query = query.Where(v => v.ArticuloId == articuloId.Value);

        query = AplicarFiltroYOrden(query, pquery);

        return await PaginarAsync(query, pquery);
    }

    public async Task<IEnumerable<ArticuloVariante>> GetStockBajoAsync()
    {
        return await _context.ArticuloVariantes
            .Include(v => v.Articulo)
            .Where(v => v.Stock <= v.StockMinimo)
            .ToListAsync();
    }
}
