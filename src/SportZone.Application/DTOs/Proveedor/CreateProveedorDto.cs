namespace SportZone.Application.DTOs.Proveedor;

public class CreateProveedorDto
{
    public string Nombre { get; set; } = string.Empty;
    public string? Contacto { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string? Direccion { get; set; }
    public string? CondicionesComerciales { get; set; }

    // Ids de las marcas que este proveedor suministra (relación N:M con Marca)
    public List<int> MarcaIds { get; set; } = new();
}
