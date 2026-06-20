namespace SportZone.Domain.Entities;

public class Marca : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? Logo { get; set; }

    public ICollection<Articulo> Articulos { get; set; } = new List<Articulo>();
}
