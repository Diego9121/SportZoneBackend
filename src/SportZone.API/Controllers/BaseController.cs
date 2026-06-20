using SportZone.Application.Common.Exceptions;

namespace SportZone.API.Controllers;

// Controlador base: estandariza el formato de TODAS las respuestas exitosas como { success, message, data }.
// Nota: la infraestructura de JWT/Login/BCrypt ya existe y funciona (ver AuthController, JwtService,
// ICurrentUserService). Solo falta volver a agregar [Authorize] aquí cuando se retome ese módulo.
[ApiController]
public abstract class BaseController : ControllerBase
{
    private static readonly string[] TiposImagenPermitidos = { "image/jpeg", "image/png", "image/webp" };
    private const long TamanoMaximoImagenBytes = 5 * 1024 * 1024; // 5 MB

    protected IActionResult RespuestaOk<T>(T datos, string mensaje = "Operación exitosa")
        => Ok(new { success = true, message = mensaje, data = datos });

    protected IActionResult RespuestaCreado<T>(T datos, string mensaje = "Recurso creado exitosamente")
        => StatusCode(201, new { success = true, message = mensaje, data = datos });

    protected IActionResult RespuestaSinDatos(string mensaje = "Operación exitosa")
        => Ok(new { success = true, message = mensaje });

    // Validación compartida para cualquier endpoint que reciba IFormFile (imágenes de Articulo, logo de Marca, etc.)
    protected static void ValidarImagen(IFormFile? archivo)
    {
        if (archivo == null || archivo.Length == 0)
            throw new ValidationException("Debes enviar un archivo de imagen");

        if (!TiposImagenPermitidos.Contains(archivo.ContentType))
            throw new ValidationException("Solo se permiten imagenes JPEG, PNG o WEBP");

        if (archivo.Length > TamanoMaximoImagenBytes)
            throw new ValidationException("La imagen no puede superar los 5 MB");
    }
}
