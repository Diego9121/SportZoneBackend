namespace SportZone.Application.DTOs.Ingreso;

// UsuarioId no se pide: lo asigna el Servicio desde ICurrentUserService (quien está autenticado al registrar)
public class CreateIngresoDto
{
    public int ProveedorId { get; set; }
    public string? NumeroDoc { get; set; }
    public DateTime? FechaDoc { get; set; }
    public string? Observacion { get; set; }
    public List<CreateIngresoDetalleDto> Detalles { get; set; } = new();
}
