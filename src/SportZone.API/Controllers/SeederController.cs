using SportZone.Application.Interfaces.Servicios;

namespace SportZone.API.Controllers;

// Endpoint de uso único: carga el contenido de Common/Data/Seeder.json a la base de datos.
[Route("api/[controller]")]
public class SeederController : BaseController
{
    private readonly IDataSeederServicio _servicio;
    private readonly IWebHostEnvironment _environment;

    public SeederController(IDataSeederServicio servicio, IWebHostEnvironment environment)
    {
        _servicio = servicio;
        _environment = environment;
    }

    [HttpPost("cargar")]
    public async Task<IActionResult> Cargar()
    {
        var ruta = Path.Combine(_environment.ContentRootPath, "Common", "Data", "Seeder.json");
        var resultado = await _servicio.CargarDesdeJsonAsync(ruta);
        return RespuestaOk(resultado, "Seed cargado correctamente");
    }
}
