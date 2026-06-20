namespace SportZone.Application.Interfaces;

// Contrato que oculta de dónde viene el usuario autenticado actual.
// Hoy lo implementa un valor fijo (Infrastructure/Services/CurrentUserService).
// Cuando exista login con JWT, se reemplaza la implementación leyendo el token - el Repository no cambia.
public interface ICurrentUserService
{
    int GetUsuarioId();
}
