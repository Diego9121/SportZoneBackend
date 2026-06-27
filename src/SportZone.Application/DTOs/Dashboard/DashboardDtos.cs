namespace SportZone.Application.DTOs.Dashboard;

// PorcentajeCambio: 100 si el mes anterior fue 0 y el actual > 0 (no hay base de comparacion real),
// 0 si ambos meses son 0. En cualquier otro caso es ((actual - anterior) / anterior) * 100.
public class DashboardVentasMesDto
{
    public int Anio { get; set; }
    public int Mes { get; set; }
    public decimal TotalMesActual { get; set; }
    public decimal TotalMesAnterior { get; set; }
    public double PorcentajeCambio { get; set; }
}

public class DashboardComprasMesDto
{
    public int Anio { get; set; }
    public int Mes { get; set; }
    public decimal TotalMesActual { get; set; }
    public decimal TotalMesAnterior { get; set; }
    public double PorcentajeCambio { get; set; }
}

public class DashboardMargenGananciaMesDto
{
    public int Anio { get; set; }
    public int Mes { get; set; }
    public decimal TotalVendido { get; set; }
    public decimal TotalCosto { get; set; }
    public decimal GananciaBruta { get; set; }
    public double MargenPorcentaje { get; set; }
}

public class DashboardParesVendidosMesDto
{
    public int Anio { get; set; }
    public int Mes { get; set; }
    public int ParesVendidos { get; set; }
}

public class DashboardVentasPorCategoriaDto
{
    public int CategoriaId { get; set; }
    public string CategoriaNombre { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public double Porcentaje { get; set; }
}

// Un punto por dia del mes consultado, para graficar compras vs ventas en el frontend
public class DashboardSerieDiariaDto
{
    public DateTime Fecha { get; set; }
    public decimal TotalVentas { get; set; }
    public decimal TotalCompras { get; set; }
}
