using SportZone.Application.DTOs.Common;

namespace SportZone.Application.Interfaces;

public interface IArticuloVarianteRepository : IRepository<ArticuloVariante>
{
    Task<ArticuloVariante?> GetByIdWithArticuloAsync(int id);

    // Soporta el escaneo de código de barras en el punto de venta
    Task<ArticuloVariante?> GetByCodigoBarrasAsync(string codigoBarras);

    // articuloId opcional: filtra "todas las variantes de un articulo". talla opcional: busca ese valor
    // en TallaUs/TallaEu/TallaUk/TallaCm (cualquiera de los 4 formatos). stock opcional: Stock >= valor.
    // query.Filter ya busca, además, por nombre del Articulo padre, precios y los campos propios de la
    // variante (color, código de barras, tallas).
    Task<(IEnumerable<ArticuloVariante> Items, int TotalCount)> GetPagedWithArticuloAsync(
        PaginacionQueryDto query, int? articuloId, string? talla, int? stock);

    // Soporta el requerimiento de "alertas automáticas cuando el stock alcance el mínimo"
    Task<IEnumerable<ArticuloVariante>> GetStockBajoAsync();
}
