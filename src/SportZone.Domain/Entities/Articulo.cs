namespace SportZone.Domain.Entities;

public class Articulo : BaseEntity
{
    public int CategoriaId { get; set; }
    public int MarcaId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? Imagen { get; set; }
    public decimal PrecioVenta { get; set; }
    public decimal PrecioCosto { get; set; }

    public Categoria Categoria { get; set; } = null!;
    public Marca Marca { get; set; } = null!;

    public ICollection<ArticuloVariante> Variantes { get; set; } = new List<ArticuloVariante>();
}
