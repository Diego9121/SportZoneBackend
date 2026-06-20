namespace SportZone.Infrastructure.Repositories;

public class IngresoRepository : Repository<Ingreso>, IIngresoRepository
{
    public IngresoRepository(ApplicationDbContext context, ICurrentUserService currentUser)
        : base(context, currentUser)
    {
    }

    public async Task<Ingreso?> GetByIdWithDetalleAsync(int id)
    {
        return await _context.Ingresos
            .Include(i => i.Proveedor)
            .Include(i => i.Usuario)
            .Include(i => i.IngresoDetalles).ThenInclude(d => d.Variante).ThenInclude(v => v.Articulo)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<(IEnumerable<Ingreso> Items, int TotalCount)> GetPagedWithDetalleAsync(PaginacionQueryDto pquery)
    {
        var query = AplicarFiltroYOrden(
            _context.Ingresos
                .Include(i => i.Proveedor)
                .Include(i => i.Usuario)
                .Include(i => i.IngresoDetalles).ThenInclude(d => d.Variante).ThenInclude(v => v.Articulo)
                .OrderByDescending(i => i.Id), // por defecto: el ingreso más reciente primero
            pquery);

        var totalCount = await query.CountAsync();
        var items = await query.Skip((pquery.Page - 1) * pquery.PageSize).Take(pquery.PageSize).ToListAsync();
        return (items, totalCount);
    }

    // Todo en un solo SaveChangesAsync -> EF Core lo ejecuta como una única transacción atómica:
    // si algo falla, ni el Ingreso ni el ajuste de stock quedan guardados a medias.
    public async Task<Ingreso> CrearConDetalleAsync(Ingreso ingreso, List<IngresoDetalle> detalles)
    {
        var usuarioId = _currentUser.GetUsuarioId();
        ingreso.CreateById = usuarioId;

        foreach (var detalle in detalles)
        {
            detalle.CreateById = usuarioId;
            ingreso.IngresoDetalles.Add(detalle);

            var variante = await _context.ArticuloVariantes.FindAsync(detalle.VarianteId);
            if (variante != null)
            {
                variante.Stock += detalle.Cantidad;
                variante.UpdateById = usuarioId;
            }
        }

        await _context.Ingresos.AddAsync(ingreso);
        await _context.SaveChangesAsync();
        return ingreso;
    }
}
