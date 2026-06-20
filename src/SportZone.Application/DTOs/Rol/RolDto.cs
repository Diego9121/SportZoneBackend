namespace SportZone.Application.DTOs.Rol;

public class RolDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public DateTime CreatedAt { get; set; }
}