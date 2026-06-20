namespace SportZone.Application.DTOs.Usuario;

// DTO de ENTRADA para actualizar. No incluye Password: el cambio de contraseña se hace en un endpoint aparte
// (no se debería poder cambiar la clave "de paso" al editar nombre o email).
public class UpdateUsuarioDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int RolId { get; set; }
    public bool Activo { get; set; }
}
