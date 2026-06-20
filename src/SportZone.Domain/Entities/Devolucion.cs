namespace SportZone.Domain.Entities;

public class Devolucion : BaseEntity
{
    public int VentaId { get; set; }
    public int UsuarioId { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public string Estado { get; set; } = "PENDIENTE";
    public string? Observacion { get; set; }

    public Venta Venta { get; set; } = null!;
    public Usuario Usuario { get; set; } = null!;

    public ICollection<DevolucionDetalle> DevolucionDetalles { get; set; } = new List<DevolucionDetalle>();
}
