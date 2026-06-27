namespace SportZone.Application.Interfaces.Servicios;

// Se ejecuta al iniciar la aplicacion (ver Program.cs), despues del seeder de productos.
// Idempotente: si ya hay Ventas o Ingresos, no hace nada.
public interface IDatosFicticiosSeederServicio
{
    Task SeedAsync();
}
