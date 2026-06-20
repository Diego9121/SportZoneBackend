namespace SportZone.Infrastructure.Repositories;

public class VentaRepository : Repository<Venta>, IVentaRepository
{
    public VentaRepository(ApplicationDbContext context, ICurrentUserService currentUser)
        : base(context, currentUser)
    {
    }

    public async Task<Venta?> GetByIdWithDetalleAsync(int id)
    {
        return await _context.Ventas
            .Include(v => v.Cliente)
            .Include(v => v.Usuario)
            .Include(v => v.VentaDetalles).ThenInclude(d => d.Variante).ThenInclude(va => va.Articulo)
            .Include(v => v.VentaPagos)
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<(IEnumerable<Venta> Items, int TotalCount)> GetPagedWithDetalleAsync(PaginacionQueryDto pquery)
    {
        var query = AplicarFiltroYOrden(
            _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Usuario)
                .Include(v => v.VentaDetalles).ThenInclude(d => d.Variante).ThenInclude(va => va.Articulo)
                .Include(v => v.VentaPagos)
                .OrderByDescending(v => v.Id), // por defecto: la venta más reciente primero
            pquery);

        var totalCount = await query.CountAsync();
        var items = await query.Skip((pquery.Page - 1) * pquery.PageSize).Take(pquery.PageSize).ToListAsync();
        return (items, totalCount);
    }

    public async Task<Venta> CrearConDetalleAsync(Venta venta, List<VentaDetalle> detalles, List<VentaPago> pagos)
    {
        var usuarioId = _currentUser.GetUsuarioId();
        venta.CreateById = usuarioId;

        foreach (var detalle in detalles)
        {
            detalle.CreateById = usuarioId;
            venta.VentaDetalles.Add(detalle);

            var variante = await _context.ArticuloVariantes.FindAsync(detalle.VarianteId);
            if (variante != null)
            {
                variante.Stock -= detalle.Cantidad;
                variante.UpdateById = usuarioId;
            }
        }

        foreach (var pago in pagos)
        {
            pago.CreateById = usuarioId;
            venta.VentaPagos.Add(pago);
        }

        await _context.Ventas.AddAsync(venta);
        await _context.SaveChangesAsync();
        return venta;
    }

    public async Task AnularConReintegroAsync(Venta venta)
    {
        var usuarioId = _currentUser.GetUsuarioId();
        venta.Estado = "ANULADA";
        venta.UpdateById = usuarioId;

        foreach (var detalle in venta.VentaDetalles)
        {
            var variante = await _context.ArticuloVariantes.FindAsync(detalle.VarianteId);
            if (variante != null)
            {
                variante.Stock += detalle.Cantidad;
                variante.UpdateById = usuarioId;
            }
        }

        _context.Ventas.Update(venta);
        await _context.SaveChangesAsync();
    }
}
