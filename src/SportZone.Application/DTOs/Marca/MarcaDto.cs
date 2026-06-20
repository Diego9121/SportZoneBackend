namespace SportZone.Application.DTOs.Marca;

public class MarcaDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? Logo { get; set; }
    public DateTime CreatedAt { get; set; }
    
}