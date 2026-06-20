using Microsoft.AspNetCore.Authorization;
using SportZone.Application.DTOs.Auth;
using SportZone.Application.Interfaces.Servicios;

namespace SportZone.API.Controllers;

[Route("api/[controller]")]
public class AuthController : BaseController
{
    private readonly IAuthServicio _servicio;

    public AuthController(IAuthServicio servicio)
    {
        _servicio = servicio;
    }

    // [AllowAnonymous] es obligatorio: BaseController exige [Authorize] por defecto,
    // y login es el único endpoint al que se debe poder llamar SIN tener todavía un token.
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var resultado = await _servicio.LoginAsync(dto);
        return RespuestaOk(resultado, "Login exitoso");
    }
}
