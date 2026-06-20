namespace SportZone.Application.Interfaces;

// Contrato de almacenamiento de imágenes. Application solo sabe que puede "subir un archivo y recibir una URL";
// no sabe que por debajo es Cloudinary — ese detalle técnico vive en Infrastructure.
public interface IImagenService
{
    Task<string> SubirImagenAsync(Stream archivo, string nombreArchivo, string carpeta);
}
