namespace SportZone.Infrastructure.Persistence.Seed;

// Representan la forma exacta del JSON (snake_case). Solo viven en Infrastructure porque
// son un detalle de implementación del seeder, no parte del contrato público de la API.
public class SeederDataDto
{
    public List<SeedMarcaDto> Marcas { get; set; } = new();
    public List<SeedCategoriaDto> Categorias { get; set; } = new();
    public List<SeedArticuloDto> Articulos { get; set; } = new();
    public List<SeedArticuloVarianteDto> ArticuloVariantes { get; set; } = new();
}

public class SeedMarcaDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? Logo { get; set; }
    public int CreateById { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class SeedCategoriaDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int CreateById { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class SeedArticuloDto
{
    public int Id { get; set; }
    public int CategoriaId { get; set; }
    public int MarcaId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? Imagen { get; set; }
    public int CreateById { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class SeedArticuloVarianteDto
{
    public int Id { get; set; }
    public int ArticuloId { get; set; }
    public string? TallaUs { get; set; }
    public string? TallaEu { get; set; }
    public string? TallaUk { get; set; }
    public string? TallaCm { get; set; }
    public string? Color { get; set; }
    public string? CodigoBarras { get; set; }
    public int Stock { get; set; }
    public int StockMinimo { get; set; }
    public decimal PrecioVenta { get; set; }
    public decimal PrecioCosto { get; set; }
    public int CreateById { get; set; }
    public DateTime CreatedAt { get; set; }
}
