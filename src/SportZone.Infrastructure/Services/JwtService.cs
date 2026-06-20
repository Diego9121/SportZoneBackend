using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace SportZone.Infrastructure.Services;

// Construye un JWT firmado con la clave secreta del appsettings. El token lleva 3 datos (claims):
// quién es (Id), su email, y su rol — eso es lo que [Authorize(Roles = "ADMIN")] verifica después.
public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerarToken(int usuarioId, string email, string rolNombre)
    {
        var secretKey = _configuration["Jwt:SecretKey"]!;
        var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credenciales = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuarioId.ToString()),
            new(ClaimTypes.Email, email),
            new(ClaimTypes.Role, rolNombre)
        };

        var horas = double.Parse(_configuration["Jwt:ExpiracionHoras"] ?? "8");

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(horas),
            signingCredentials: credenciales);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
