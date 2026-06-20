using SportZone.Application.DTOs.Common;

namespace SportZone.Application.Interfaces;

// Extiende el contrato genérico con operaciones que necesitan conocer la relación Usuario -> Rol (1:N).
// El repositorio genérico no puede hacer "Include(Rol)" porque no sabe qué relaciones tiene cada entidad.
public interface IUsuarioRepository : IRepository<Usuario>
{
    Task<Usuario?> GetByIdWithRolAsync(int id);
    Task<Usuario?> GetByEmailWithRolAsync(string email);
    Task<(IEnumerable<Usuario> Items, int TotalCount)> GetPagedWithRolAsync(PaginacionQueryDto query);
    Task<bool> ExisteEmailAsync(string email);
}
