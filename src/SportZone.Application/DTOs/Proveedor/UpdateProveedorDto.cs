namespace SportZone.Application.DTOs.Proveedor;

public class UpdateProveedorDto
{
    public string Nombre { get; set; } = string.Empty;
    public string? Contacto { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string? Direccion { get; set; }
    public string? CondicionesComerciales { get; set; }

    // Reemplaza completamente la lista de marcas asociadas (las que falten se quitan, las nuevas se agregan)
    public List<int> MarcaIds { get; set; } = new();
}
