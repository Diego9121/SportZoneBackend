using SportZone.Application.DTOs.Common;

namespace SportZone.Application.Interfaces;

public interface IIngresoRepository : IRepository<Ingreso>
{
    Task<Ingreso?> GetByIdWithDetalleAsync(int id);
    Task<(IEnumerable<Ingreso> Items, int TotalCount)> GetPagedWithDetalleAsync(PaginacionQueryDto query);

    // Crea el Ingreso + sus detalles e INCREMENTA el stock de cada variante en una sola transacción
    Task<Ingreso> CrearConDetalleAsync(Ingreso ingreso, List<IngresoDetalle> detalles);
}
