namespace SportZone.Domain.Entities;

public class Ingreso : BaseEntity
{
    public int ProveedorId { get; set; }
    public int UsuarioId { get; set; }
    public string? NumeroDoc { get; set; }
    public DateTime? FechaDoc { get; set; }
    public decimal Total { get; set; }
    public string? Observacion { get; set; }

    public Proveedor Proveedor { get; set; } = null!;
    public Usuario Usuario { get; set; } = null!;

    public ICollection<IngresoDetalle> IngresoDetalles { get; set; } = new List<IngresoDetalle>();
}
