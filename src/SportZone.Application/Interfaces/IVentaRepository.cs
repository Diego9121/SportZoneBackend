using SportZone.Application.DTOs.Common;

namespace SportZone.Application.Interfaces;

public interface IVentaRepository : IRepository<Venta>
{
    Task<Venta?> GetByIdWithDetalleAsync(int id);
    Task<(IEnumerable<Venta> Items, int TotalCount)> GetPagedWithDetalleAsync(PaginacionQueryDto query);

    // Crea Venta + detalle y DECREMENTA el stock de cada variante, todo en una transacción
    Task<Venta> CrearConDetalleAsync(Venta venta, List<VentaDetalle> detalles);

    // Marca la venta como ANULADA y REINTEGRA el stock vendido
    Task AnularConReintegroAsync(Venta venta);
}
