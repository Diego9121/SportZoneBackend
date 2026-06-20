using SportZone.Application.Common.Exceptions;
using SportZone.Application.DTOs.Auth;
using SportZone.Application.Interfaces;
using SportZone.Application.Interfaces.Servicios;

namespace SportZone.Application.Servicios;

public class AuthServicio : IAuthServicio
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IJwtService _jwtService;
    private const double ExpiracionHoras = 8;

    public AuthServicio(IUsuarioRepository usuarioRepository, IJwtService jwtService)
    {
        _usuarioRepository = usuarioRepository;
        _jwtService = jwtService;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
    {
        var usuario = await _usuarioRepository.GetByEmailWithRolAsync(dto.Email);

        // Mismo mensaje genérico tanto si el email no existe como si la contraseña está mal:
        // así no le decimos a un atacante "el email sí existe, solo falló la contraseña".
        if (usuario == null || !usuario.Activo || !BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash))
            throw new ValidationException("Credenciales invalidas");

        var token = _jwtService.GenerarToken(usuario.Id, usuario.Email, usuario.Rol.Nombre);

        usuario.UltimoAcceso = DateTime.UtcNow;
        await _usuarioRepository.UpdateAsync(usuario);

        return new LoginResponseDto
        {
            Token = token,
            UsuarioId = usuario.Id,
            Nombre = usuario.Nombre,
            Email = usuario.Email,
            RolNombre = usuario.Rol.Nombre,
            ExpiraEn = DateTime.UtcNow.AddHours(ExpiracionHoras)
        };
    }
}
