namespace SportZone.Application.DTOs.Usuario;

// DTO de SALIDA: lo que ve el cliente. RolNombre viene "aplanado" desde la relación 1:N con Rol,
// así el frontend no tiene que pedir el Rol por separado.
public class UsuarioDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public DateTime? UltimoAcceso { get; set; }
    public int RolId { get; set; }
    public string RolNombre { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
