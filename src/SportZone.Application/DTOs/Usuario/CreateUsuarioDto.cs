namespace SportZone.Application.DTOs.Usuario;

// DTO de ENTRADA para crear. Password llega en texto plano por HTTPS; el Servicio lo hashea antes de guardar.
public class CreateUsuarioDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int RolId { get; set; }
}
