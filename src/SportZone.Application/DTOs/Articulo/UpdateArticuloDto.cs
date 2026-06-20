namespace SportZone.Application.DTOs.Articulo;

public class UpdateArticuloDto
{
    public int CategoriaId { get; set; }
    public int MarcaId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? Imagen { get; set; }
}
