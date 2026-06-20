namespace SportZone.API.Controllers;

// Controlador base: estandariza el formato de TODAS las respuestas exitosas como { success, message, data }.
// Nota: la infraestructura de JWT/Login/BCrypt ya existe y funciona (ver AuthController, JwtService,
// ICurrentUserService). Solo falta volver a agregar [Authorize] aquí cuando se retome ese módulo.
[ApiController]
public abstract class BaseController : ControllerBase
{
    protected IActionResult RespuestaOk<T>(T datos, string mensaje = "Operación exitosa")
        => Ok(new { success = true, message = mensaje, data = datos });

    protected IActionResult RespuestaCreado<T>(T datos, string mensaje = "Recurso creado exitosamente")
        => StatusCode(201, new { success = true, message = mensaje, data = datos });

    protected IActionResult RespuestaSinDatos(string mensaje = "Operación exitosa")
        => Ok(new { success = true, message = mensaje });
}
