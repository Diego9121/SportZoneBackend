namespace SportZone.Domain.Entities;

public class Cliente : BaseEntity
{
    public string? TipoDocumento { get; set; }
    public string? Documento { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string? Direccion { get; set; }
    public int PuntosFidelizacion { get; set; } = 0;
    public decimal DescuentoFidelizacion { get; set; } = 0;

    public ICollection<Venta> Ventas { get; set; } = new List<Venta>();
}
