using SportZone.Application.DTOs.Common;

namespace SportZone.Application.Interfaces;

// Necesita Include(Categoria), Include(Marca) e Include(Variantes) para los campos aplanados del DTO
public interface IArticuloRepository : IRepository<Articulo>
{
    Task<Articulo?> GetByIdWithDetallesAsync(int id);
    Task<(IEnumerable<Articulo> Items, int TotalCount)> GetPagedWithDetallesAsync(PaginacionQueryDto query);
}
