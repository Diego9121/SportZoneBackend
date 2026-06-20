namespace SportZone.Application.DTOs.Venta;

// Sin PrecioUnitario a propósito: el precio lo calcula el Servidor desde la Variante,
// nunca se confía en un precio que mande el cliente (evita manipulación de precios).
public class CreateVentaDetalleDto
{
    public int VarianteId { get; set; }
    public int Cantidad { get; set; }
    public decimal Descuento { get; set; } = 0;
}
