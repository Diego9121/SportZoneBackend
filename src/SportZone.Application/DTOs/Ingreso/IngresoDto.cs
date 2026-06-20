namespace SportZone.Application.DTOs.Ingreso;

public class IngresoDto
{
    public int Id { get; set; }
    public int ProveedorId { get; set; }
    public string ProveedorNombre { get; set; } = string.Empty;
    public string? NumeroDoc { get; set; }
    public decimal Total { get; set; }
    public string? Observacion { get; set; }
    public List<IngresoDetalleDto> Detalles { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}
