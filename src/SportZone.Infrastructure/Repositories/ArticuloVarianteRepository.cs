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
        PaginacionQueryDto pquery, int? articuloId, string? talla, int? stock)
    {
        var query = _context.ArticuloVariantes.Include(v => v.Articulo).AsQueryable();

        if (articuloId.HasValue)
            query = query.Where(v => v.ArticuloId == articuloId.Value);

        // Filtro de texto propio (no se usa el genérico por Reflection): además de los campos de la
        // variante, también busca por el nombre del Articulo padre y por los precios (convertidos a texto),
        // algo que el genérico no puede hacer porque Articulo es navegación y los precios son decimal, no string.
        if (!string.IsNullOrWhiteSpace(pquery.Filter))
        {
            var filtro = pquery.Filter;
            query = query.Where(v =>
                (v.Articulo != null && v.Articulo.Nombre.Contains(filtro)) ||
                (v.TallaUs != null && v.TallaUs.Contains(filtro)) ||
                (v.TallaEu != null && v.TallaEu.Contains(filtro)) ||
                (v.TallaUk != null && v.TallaUk.Contains(filtro)) ||
                (v.TallaCm != null && v.TallaCm.Contains(filtro)) ||
                (v.Color != null && v.Color.Contains(filtro)) ||
                (v.CodigoBarras != null && v.CodigoBarras.Contains(filtro)) ||
                v.PrecioVenta.ToString().Contains(filtro) ||
                v.PrecioCosto.ToString().Contains(filtro));
        }

        // Búsqueda dedicada por talla: el valor puede venir en cualquiera de los 4 formatos (US/EU/UK/CM)
        if (!string.IsNullOrWhiteSpace(talla))
        {
            query = query.Where(v =>
                (v.TallaUs != null && v.TallaUs.Contains(talla)) ||
                (v.TallaEu != null && v.TallaEu.Contains(talla)) ||
                (v.TallaUk != null && v.TallaUk.Contains(talla)) ||
                (v.TallaCm != null && v.TallaCm.Contains(talla)));
        }

        // Búsqueda dedicada por stock: NO es igualdad, es "al menos este stock" (ej. stock=5 -> Stock >= 5)
        if (stock.HasValue)
            query = query.Where(v => v.Stock >= stock.Value);

        if (!string.IsNullOrWhiteSpace(pquery.SortBy))
            query = AplicarOrden(query, pquery.SortBy, pquery.SortDirection);

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
