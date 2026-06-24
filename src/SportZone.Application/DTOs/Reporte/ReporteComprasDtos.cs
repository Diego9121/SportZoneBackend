namespace SportZone.Application.DTOs.Reporte;

public class ReporteComprasResumenDto
{
    public DateTime? Desde { get; set; }
    public DateTime? Hasta { get; set; }
    public int CantidadIngresos { get; set; }
    public decimal TotalComprado { get; set; }
    public decimal CompraPromedio { get; set; }
}

public class ReporteComprasPorProveedorDto
{
    public int ProveedorId { get; set; }
    public string ProveedorNombre { get; set; } = string.Empty;
    public int CantidadIngresos { get; set; }
    public decimal TotalComprado { get; set; }
}

// Listado completo de compras (ingresos) del período + el resumen agregado
public class ReporteComprasDetalladoDto
{
    public ReporteComprasResumenDto Resumen { get; set; } = new();
    public List<ReporteCompraItemDto> Compras { get; set; } = new();
}

public class ReporteCompraItemDto
{
    public int Id { get; set; }
    public string? NumeroDoc { get; set; }
    public string ProveedorNombre { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<ReporteCompraDetalleItemDto> Detalles { get; set; } = new();
}

public class ReporteCompraDetalleItemDto
{
    public string Nombre { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Total { get; set; }
}
