using SportZone.Application.DTOs.Auth;

namespace SportZone.Application.Interfaces.Servicios;

public interface IAuthServicio
{
    Task<LoginResponseDto> LoginAsync(LoginDto dto);
}
