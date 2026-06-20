using SportZone.Application.DTOs.Common;

namespace SportZone.Application.Interfaces;

public interface IArticuloVarianteRepository : IRepository<ArticuloVariante>
{
    Task<ArticuloVariante?> GetByIdWithArticuloAsync(int id);

    // Soporta el escaneo de código de barras en el punto de venta
    Task<ArticuloVariante?> GetByCodigoBarrasAsync(string codigoBarras);

    // articuloId opcional: permite filtrar "todas las variantes de un articulo" desde el mismo endpoint
    Task<(IEnumerable<ArticuloVariante> Items, int TotalCount)> GetPagedWithArticuloAsync(PaginacionQueryDto query, int? articuloId);

    // Soporta el requerimiento de "alertas automáticas cuando el stock alcance el mínimo"
    Task<IEnumerable<ArticuloVariante>> GetStockBajoAsync();
}
