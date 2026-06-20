namespace SportZone.Domain.Entities;

public class Proveedor : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string? Contacto { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string? Direccion { get; set; }

    public ICollection<Ingreso> Ingresos { get; set; } = new List<Ingreso>();
}
