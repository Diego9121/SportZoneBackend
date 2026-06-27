namespace SportZone.Application.Interfaces.Servicios;

// Se ejecuta al iniciar la aplicacion (ver Program.cs). Idempotente: si ya hay Roles, no hace nada.
public interface IUsuarioSeederServicio
{
    Task SeedAsync();
}
