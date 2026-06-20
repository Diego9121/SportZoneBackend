using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace SportZone.Infrastructure.Services;

// Lee el Id del usuario autenticado desde los claims del JWT (ya validado por el middleware de autenticación
// antes de llegar aquí). Si no hay nadie autenticado (caso límite, no debería pasar detrás de [Authorize]),
// cae al usuario Id 1 como reserva para no romper la auditoría.
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int GetUsuarioId()
    {
        var claim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null && int.TryParse(claim.Value, out var id) ? id : 1;
    }
}
