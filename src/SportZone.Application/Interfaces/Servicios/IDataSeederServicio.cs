namespace SportZone.Application.Interfaces.Servicios;

// Carga masiva única de datos de prueba desde un archivo JSON. No es un CRUD normal:
// solo se usa una vez, al preparar el ambiente de demo/defensa.
public interface IDataSeederServicio
{
    Task<string> CargarDesdeJsonAsync(string rutaArchivoJson);
}
