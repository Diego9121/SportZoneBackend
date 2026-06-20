using SportZone.Application.DTOs.Common;

namespace SportZone.Application.Interfaces;

public interface IProveedorRepository : IRepository<Proveedor>
{
    Task<Proveedor?> GetByIdWithMarcasAsync(int id);
    Task<(IEnumerable<Proveedor> Items, int TotalCount)> GetPagedWithMarcasAsync(PaginacionQueryDto query);

    // Reemplaza el conjunto completo de marcas asociadas: agrega las nuevas, quita (soft delete) las que ya no están
    Task SincronizarMarcasAsync(int proveedorId, List<int> marcaIds);
}
