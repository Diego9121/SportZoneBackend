namespace SportZone.Application.DTOs.Auth;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public int UsuarioId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string RolNombre { get; set; } = string.Empty;
    public DateTime ExpiraEn { get; set; }
}
