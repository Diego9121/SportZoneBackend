namespace SportZone.Application.DTOs.ArticuloVariante;

// Para correcciones manuales de inventario (ej. diferencia detectada en un conteo físico de bodega)
public class AjustarStockDto
{
    public int Cantidad { get; set; } // positivo suma, negativo resta
    public string Motivo { get; set; } = string.Empty;
}
