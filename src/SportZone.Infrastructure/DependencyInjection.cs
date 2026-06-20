using SportZone.Infrastructure.Services;

namespace SportZone.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Permite a CurrentUserService leer el HttpContext de la petición actual (donde vive el token validado)
        services.AddHttpContextAccessor();

        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IArticuloRepository, ArticuloRepository>();
        services.AddScoped<IArticuloVarianteRepository, ArticuloVarianteRepository>();
        services.AddScoped<IProveedorRepository, ProveedorRepository>();
        services.AddScoped<IIngresoRepository, IngresoRepository>();
        services.AddScoped<IVentaRepository, VentaRepository>();

        return services;
    }
}
