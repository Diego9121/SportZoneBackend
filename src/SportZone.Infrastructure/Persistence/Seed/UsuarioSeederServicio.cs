using SportZone.Application.Interfaces.Servicios;

namespace SportZone.Infrastructure.Persistence.Seed;

public class UsuarioSeederServicio : IUsuarioSeederServicio
{
    private readonly ApplicationDbContext _context;

    public UsuarioSeederServicio(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        if (await _context.Roles.AnyAsync())
            return;

        var administrador = new Rol { Nombre = "Administrador", Descripcion = "Acceso a todo el sistema" };
        var vendedor = new Rol { Nombre = "Vendedor", Descripcion = "Tiene acceso a ventas y almacenes" };
        var almacenes = new Rol { Nombre = "Almacenes", Descripcion = "Tiene acceso a compras, almacenes y reportes" };

        await _context.Roles.AddRangeAsync(administrador, vendedor, almacenes);
        await _context.SaveChangesAsync();

        if (await _context.Usuarios.AnyAsync())
            return;

        await _context.Usuarios.AddAsync(new Usuario
        {
            RolId = administrador.Id,
            Nombre = "Diego",
            Email = "diego@gmail.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            Activo = true
        });
        await _context.SaveChangesAsync();
    }
}
