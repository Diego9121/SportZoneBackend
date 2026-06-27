using Microsoft.Extensions.DependencyInjection;
using SportZone.Application.Interfaces.Servicios;
using SportZone.Application.Servicios;

namespace SportZone.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICategoriaServicio, CategoriaServicio>();
        services.AddScoped<IMarcaServicio, MarcaServicio>();
        services.AddScoped<IRolServicio, RolServicio>();
        services.AddScoped<IUsuarioServicio, UsuarioServicio>();
        services.AddScoped<IArticuloServicio, ArticuloServicio>();
        services.AddScoped<IArticuloVarianteServicio, ArticuloVarianteServicio>();
        services.AddScoped<IClienteServicio, ClienteServicio>();
        services.AddScoped<IProveedorServicio, ProveedorServicio>();
        services.AddScoped<IIngresoServicio, IngresoServicio>();
        services.AddScoped<IVentaServicio, VentaServicio>();
        services.AddScoped<IAuthServicio, AuthServicio>();
        services.AddScoped<IMovimientoStockServicio, MovimientoStockServicio>();
        services.AddScoped<IReporteServicio, ReporteServicio>();
        services.AddScoped<IDashboardServicio, DashboardServicio>();

        return services;
    }
}
