using SportZone.Application.Interfaces;

namespace SportZone.API.Controllers;

// Endpoint genérico de subida: cualquier formulario del frontend (Articulo, Marca, lo que sea
// que necesite una imagen a futuro) sube el archivo aquí, recibe la URL, y la manda en el
// Create/Update normal de ese módulo (CreateArticuloDto.Imagen, CreateMarcaDto.Logo, etc.).
[Route("api/[controller]")]
public class ImagenesController : BaseController
{
    private readonly IImagenService _imagenService;

    public ImagenesController(IImagenService imagenService)
    {
        _imagenService = imagenService;
    }

    // POST /api/Imagenes?carpeta=articulos  (carpeta es opcional, agrupa dentro de Cloudinary)
    [HttpPost]
    [RequestSizeLimit(5_000_000)]
    public async Task<IActionResult> Subir(IFormFile archivo, [FromQuery] string carpeta = "general")
    {
        ValidarImagen(archivo);

        using var stream = archivo.OpenReadStream();
        var url = await _imagenService.SubirImagenAsync(stream, archivo.FileName, $"sportzone/{carpeta}");

        return RespuestaOk(new { url }, "Imagen subida correctamente");
    }
}
