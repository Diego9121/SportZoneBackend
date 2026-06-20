namespace SportZone.Infrastructure.Repositories;

public class ProveedorRepository : Repository<Proveedor>, IProveedorRepository
{
    public ProveedorRepository(ApplicationDbContext context, ICurrentUserService currentUser)
        : base(context, currentUser)
    {
    }

    public async Task<Proveedor?> GetByIdWithMarcasAsync(int id)
    {
        return await _context.Proveedores
            .Include(p => p.ProveedorMarcas).ThenInclude(pm => pm.Marca)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<(IEnumerable<Proveedor> Items, int TotalCount)> GetPagedWithMarcasAsync(PaginacionQueryDto pquery)
    {
        var query = AplicarFiltroYOrden(
            _context.Proveedores.Include(p => p.ProveedorMarcas).ThenInclude(pm => pm.Marca).AsQueryable(),
            pquery);

        var totalCount = await query.CountAsync();
        var items = await query.Skip((pquery.Page - 1) * pquery.PageSize).Take(pquery.PageSize).ToListAsync();
        return (items, totalCount);
    }

    // Bypassa el CRUD genérico porque maneja varias filas de ProveedorMarca a la vez;
    // por eso asigna CreateById/DeleteById manualmente en lugar de dejarlo al Repository<T>.
    public async Task SincronizarMarcasAsync(int proveedorId, List<int> marcaIds)
    {
        var actuales = await _context.ProveedorMarcas
            .Where(pm => pm.ProveedorId == proveedorId)
            .ToListAsync();

        var usuarioId = _currentUser.GetUsuarioId();

        var aEliminar = actuales.Where(pm => !marcaIds.Contains(pm.MarcaId)).ToList();
        foreach (var pm in aEliminar)
            pm.DeleteById = usuarioId;
        if (aEliminar.Count > 0)
            _context.ProveedorMarcas.RemoveRange(aEliminar);

        var existentesIds = actuales.Select(pm => pm.MarcaId).ToHashSet();
        var nuevas = marcaIds
            .Where(mid => !existentesIds.Contains(mid))
            .Select(mid => new ProveedorMarca { ProveedorId = proveedorId, MarcaId = mid, CreateById = usuarioId })
            .ToList();

        if (nuevas.Count > 0)
            await _context.ProveedorMarcas.AddRangeAsync(nuevas);

        await _context.SaveChangesAsync();
    }
}
