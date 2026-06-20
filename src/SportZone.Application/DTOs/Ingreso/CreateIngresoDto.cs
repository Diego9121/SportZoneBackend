namespace SportZone.Application.DTOs.Ingreso;

public class CreateIngresoDto
{
    public int ProveedorId { get; set; }
    public string? NumeroDoc { get; set; }
    public string? Observacion { get; set; }
    public List<CreateIngresoDetalleDto> Detalles { get; set; } = new();
}
