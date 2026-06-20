namespace SportZone.Infrastructure.Repositories;

// Hereda TODO el CRUD genérico de Repository<Usuario> y añade consultas que necesitan
// conocer la relación Usuario -> Rol (Include), algo que el genérico no puede saber hacer.
public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(ApplicationDbContext context, ICurrentUserService currentUser)
        : base(context, currentUser)
    {
    }

    public async Task<Usuario?> GetByIdWithRolAsync(int id)
    {
        return await _context.Usuarios.Include(u => u.Rol).FirstOrDefaultAsync(u => u.Id == id);
    }

    // La usa AuthServicio para el login: necesita el Rol cargado para meterlo como claim en el JWT
    public async Task<Usuario?> GetByEmailWithRolAsync(string email)
    {
        return await _context.Usuarios.Include(u => u.Rol).FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<(IEnumerable<Usuario> Items, int TotalCount)> GetPagedWithRolAsync(PaginacionQueryDto pquery)
    {
        var query = AplicarFiltroYOrden(_context.Usuarios.Include(u => u.Rol).OrderBy(u => u.Id), pquery);
        var totalCount = await query.CountAsync();
        var items = await query.Skip((pquery.Page - 1) * pquery.PageSize).Take(pquery.PageSize).ToListAsync();
        return (items, totalCount);
    }

    public async Task<bool> ExisteEmailAsync(string email)
    {
        return await _context.Usuarios.AnyAsync(u => u.Email == email);
    }
}
