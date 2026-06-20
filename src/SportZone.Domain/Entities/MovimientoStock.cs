namespace SportZone.Domain.Entities;

// Registro de cada entrada o salida de stock de una variante; enlaza opcionalmente con el
// Ingreso o la Venta que lo originó (los dos son mutuamente excluyentes en la práctica).
public class MovimientoStock : BaseEntity
{
    public int ArticuloVarianteId { get; set; }
    public int? IngresoId { get; set; }
    public int? VentaId { get; set; }
    public string TipoMovimiento { get; set; } = string.Empty; // "ENTRADA" | "SALIDA"
    public int Cantidad { get; set; }
    public string? NumeroDoc { get; set; }

    public ArticuloVariante ArticuloVariante { get; set; } = null!;
    public Ingreso? Ingreso { get; set; }
    public Venta? Venta { get; set; }
}
