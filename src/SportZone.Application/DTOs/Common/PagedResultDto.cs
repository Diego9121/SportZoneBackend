namespace SportZone.Application.DTOs.Common;

// Envoltorio genérico de respuesta paginada: sirve para CUALQUIER entidad (Rol, Usuario, Articulo...)
public class PagedResultDto<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }

    // Calculado, no se guarda en ningún lado: total de páginas según el conteo y tamaño de página
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;
}
