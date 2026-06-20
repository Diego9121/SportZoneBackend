namespace SportZone.Application.DTOs.Venta;

// UsuarioId no se pide: lo asigna el Servicio desde ICurrentUserService.
// NumeroDoc y Total tampoco: los calcula el Servicio.
public class CreateVentaDto
{
    public int? ClienteId { get; set; }
    public string TipoComprobante { get; set; } = "RECIBO"; // RECIBO | FACTURA
    public string? Observacion { get; set; }
    public List<CreateVentaDetalleDto> Detalles { get; set; } = new();
}
