namespace SportZone.Application.DTOs.Common;

// DTO de entrada para listar con paginación. Llega desde la URL: GET /api/Roles?page=2&pageSize=20
public class PaginacionQueryDto
{
    private int _page = 1;
    private int _pageSize = 10;

    // El setter se "autodefiende": nunca permite página menor a 1
    public int Page
    {
        get => _page;
        set => _page = value < 1 ? 1 : value;
    }

    // El setter limita el tamaño entre 1 y 100 para evitar que alguien pida 1 millón de filas
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value < 1 ? 10 : (value > 100 ? 100 : value);
    }

    // Texto libre: busca en todas las propiedades de tipo string de la entidad (excepto las sensibles)
    public string? Filter { get; set; }

    // Nombre de la propiedad por la que ordenar (ej. "Nombre", "CreatedAt"). Si no se manda, se ordena por Id.
    public string? SortBy { get; set; }

    // "asc" | "desc". Cualquier otro valor (o ausencia) se trata como "asc".
    public string? SortDirection { get; set; }
}
