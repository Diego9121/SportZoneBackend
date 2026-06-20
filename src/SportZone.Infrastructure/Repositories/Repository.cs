using SportZone.Application.DTOs.Common;

namespace SportZone.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    // Propiedades de texto que NUNCA deben entrar en el filtro genérico (ej. password hasheado)
    private static readonly string[] CamposExcluidosDelFiltro = { "PasswordHash", "TokenRefresh" };

    // protected: las clases específicas (ej. UsuarioRepository) heredan y reutilizan este campo
    protected readonly ApplicationDbContext _context;
    protected readonly ICurrentUserService _currentUser;

    public Repository(ApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _context.Set<T>().ToListAsync();
    }

    // Cuenta el total ANTES de paginar, luego aplica filtro/orden/Skip/Take.
    public async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(PaginacionQueryDto query)
    {
        var consulta = AplicarFiltroYOrden(_context.Set<T>().OrderBy(e => e.Id), query);
        var totalCount = await consulta.CountAsync();
        var items = await consulta.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).ToListAsync();
        return (items, totalCount);
    }

    public async Task<bool> ExisteAsync(Expression<Func<T, bool>> predicado)
    {
        return await _context.Set<T>().AnyAsync(predicado);
    }

    // Antes este valor se hardcodeaba como 1 en cada Servicio; ahora lo asigna el Repository una sola vez
    public async Task<T> CreateAsync(T entity)
    {
        entity.CreateById = _currentUser.GetUsuarioId();
        await _context.Set<T>().AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<T> UpdateAsync(T entity)
    {
        entity.UpdateById = _currentUser.GetUsuarioId();
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Set<T>().FindAsync(id);
        if (entity != null)
        {
            entity.DeleteById = _currentUser.GetUsuarioId();
            _context.Set<T>().Remove(entity); // Remove() -> SaveChangesAsync lo convierte en soft delete
            await _context.SaveChangesAsync();
        }
    }

    // Punto de extensión: los repositorios específicos (UsuarioRepository, ArticuloRepository, etc.)
    // llaman a este mismo método sobre su propio IQueryable (con sus Include + su orden por defecto) antes de paginar.
    // Si no se pide "sortBy" explícito, se respeta el orden que ya traía la consulta (ej. más reciente primero).
    protected static IQueryable<T> AplicarFiltroYOrden(IQueryable<T> query, PaginacionQueryDto pquery)
    {
        if (!string.IsNullOrWhiteSpace(pquery.Filter))
            query = AplicarFiltroTexto(query, pquery.Filter);

        if (!string.IsNullOrWhiteSpace(pquery.SortBy))
            query = AplicarOrden(query, pquery.SortBy, pquery.SortDirection);

        return query;
    }

    // Busca el texto en TODAS las propiedades string de la entidad (Nombre, Codigo, Email, etc.)
    // usando Expression Trees: arma dinámicamente "x.Prop1.Contains(filtro) OR x.Prop2.Contains(filtro) OR ..."
    private static IQueryable<T> AplicarFiltroTexto(IQueryable<T> query, string filtro)
    {
        var propiedadesTexto = typeof(T).GetProperties()
            .Where(p => p.PropertyType == typeof(string) && !CamposExcluidosDelFiltro.Contains(p.Name))
            .ToList();

        if (propiedadesTexto.Count == 0)
            return query;

        var parametro = Expression.Parameter(typeof(T), "x");
        var metodoContains = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!;
        Expression? combinado = null;

        foreach (var propiedad in propiedadesTexto)
        {
            var acceso = Expression.Property(parametro, propiedad);
            var noNulo = Expression.NotEqual(acceso, Expression.Constant(null, typeof(string)));
            var contiene = Expression.Call(acceso, metodoContains, Expression.Constant(filtro));
            var seguro = Expression.AndAlso(noNulo, contiene);

            combinado = combinado == null ? seguro : Expression.OrElse(combinado, seguro);
        }

        var lambda = Expression.Lambda<Func<T, bool>>(combinado!, parametro);
        return query.Where(lambda);
    }

    // Ordena dinámicamente por el nombre de propiedad recibido desde la URL (?sortBy=Nombre&sortDirection=desc)
    private static IQueryable<T> AplicarOrden(IQueryable<T> query, string sortBy, string? sortDirection)
    {
        var propiedad = typeof(T).GetProperty(sortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        if (propiedad == null)
            return query.OrderBy(e => e.Id);

        var parametro = Expression.Parameter(typeof(T), "x");
        var acceso = Expression.Property(parametro, propiedad);
        var convertido = Expression.Convert(acceso, typeof(object));
        var lambda = Expression.Lambda<Func<T, object>>(convertido, parametro);

        return string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase)
            ? query.OrderByDescending(lambda)
            : query.OrderBy(lambda);
    }
}
