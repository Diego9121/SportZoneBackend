namespace SportZone.Domain.Entities;

public class Proveedor : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string? Contacto { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string? Direccion { get; set; }
    public string? CondicionesComerciales { get; set; }

    public ICollection<ProveedorMarca> ProveedorMarcas { get; set; } = new List<ProveedorMarca>();
    public ICollection<Ingreso> Ingresos { get; set; } = new List<Ingreso>();
}
