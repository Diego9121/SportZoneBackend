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
            .Include(v => v.VentaDetalles).ThenInclude(d => d.Variante).ThenInclude(va => va.Articulo)
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<(IEnumerable<Venta> Items, int TotalCount)> GetPagedWithDetalleAsync(PaginacionQueryDto pquery)
    {
        var query = AplicarFiltroYOrden(
            _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.VentaDetalles).ThenInclude(d => d.Variante).ThenInclude(va => va.Articulo)
                .OrderByDescending(v => v.Id), // por defecto: la venta más reciente primero
            pquery);

        var totalCount = await query.CountAsync();
        var items = await query.Skip((pquery.Page - 1) * pquery.PageSize).Take(pquery.PageSize).ToListAsync();
        return (items, totalCount);
    }

    public async Task<Venta> CrearConDetalleAsync(Venta venta, List<VentaDetalle> detalles)
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

            // Misma idea que en Ingreso: al venir de la colección de navegación, EF Core asigna el VentaId solo
            venta.MovimientosStock.Add(new MovimientoStock
            {
                ArticuloVarianteId = detalle.VarianteId,
                TipoMovimiento = "SALIDA",
                Cantidad = detalle.Cantidad,
                NumeroDoc = venta.NumeroDoc,
                CreateById = usuarioId
            });
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

            // La venta ya existe (tiene Id real), así que aquí sí se asigna VentaId explícitamente.
            // Es un "ENTRADA" porque el stock vuelve a la bodega al anular la venta.
            await _context.MovimientosStock.AddAsync(new MovimientoStock
            {
                ArticuloVarianteId = detalle.VarianteId,
                VentaId = venta.Id,
                TipoMovimiento = "ENTRADA",
                Cantidad = detalle.Cantidad,
                NumeroDoc = venta.NumeroDoc,
                CreateById = usuarioId
            });
        }

        _context.Ventas.Update(venta);
        await _context.SaveChangesAsync();
    }
}
