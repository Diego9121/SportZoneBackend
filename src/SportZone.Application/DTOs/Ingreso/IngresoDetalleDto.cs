namespace SportZone.Application.DTOs.Ingreso;

public class IngresoDetalleDto
{
    public int Id { get; set; }
    public int VarianteId { get; set; }
    public string VarianteDescripcion { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioCosto { get; set; }
    public decimal Subtotal { get; set; }
}
