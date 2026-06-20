using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace SportZone.Infrastructure.Services;

public class CloudinaryImagenService : IImagenService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryImagenService(IConfiguration configuration)
    {
        var cuenta = new Account(
            configuration["Cloudinary:CloudName"],
            configuration["Cloudinary:ApiKey"],
            configuration["Cloudinary:ApiSecret"]);

        _cloudinary = new Cloudinary(cuenta);
    }

    public async Task<string> SubirImagenAsync(Stream archivo, string nombreArchivo, string carpeta)
    {
        var parametros = new ImageUploadParams
        {
            File = new FileDescription(nombreArchivo, archivo),
            Folder = carpeta
        };

        var resultado = await _cloudinary.UploadAsync(parametros);

        if (resultado.Error != null)
            throw new InvalidOperationException($"Error al subir imagen a Cloudinary: {resultado.Error.Message}");

        return resultado.SecureUrl.ToString();
    }
}
