namespace SportZone.Application.Interfaces;

// Contrato: Application solo sabe que puede pedir un token. No sabe que por debajo es JWT firmado con HMAC-SHA256;
// ese detalle técnico vive en Infrastructure (JwtService).
public interface IJwtService
{
    string GenerarToken(int usuarioId, string email, string rolNombre);
}
