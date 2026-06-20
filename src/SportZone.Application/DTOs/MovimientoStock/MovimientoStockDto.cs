namespace SportZone.Application.DTOs.MovimientoStock;

public class MovimientoStockDto
{
    public int Id { get; set; }
    public int ArticuloVarianteId { get; set; }
    public string ArticuloVarianteDescripcion { get; set; } = string.Empty;
    public int? IngresoId { get; set; }
    public int? VentaId { get; set; }
    public string TipoMovimiento { get; set; } = string.Empty; // ENTRADA | SALIDA
    public int Cantidad { get; set; }
    public string? NumeroDoc { get; set; }
    public DateTime CreatedAt { get; set; }
}
