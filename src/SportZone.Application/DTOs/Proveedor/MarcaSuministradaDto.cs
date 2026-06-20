namespace SportZone.Application.DTOs.Proveedor;

// Item anidado dentro de ProveedorDto: representa una marca que ese proveedor suministra
public class MarcaSuministradaDto
{
    public int MarcaId { get; set; }
    public string MarcaNombre { get; set; } = string.Empty;
}
