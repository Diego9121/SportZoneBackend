namespace SportZone.Domain.Entities;

// Ya no referencia Usuario directamente: CreateById (heredado de BaseEntity) ya identifica quién lo registró.
public class Ingreso : BaseEntity
{
    public int ProveedorId { get; set; }
    public string? NumeroDoc { get; set; }
    public decimal Total { get; set; }
    public string? Observacion { get; set; }

    public Proveedor Proveedor { get; set; } = null!;

    public ICollection<IngresoDetalle> IngresoDetalles { get; set; } = new List<IngresoDetalle>();
    public ICollection<MovimientoStock> MovimientosStock { get; set; } = new List<MovimientoStock>();
}
