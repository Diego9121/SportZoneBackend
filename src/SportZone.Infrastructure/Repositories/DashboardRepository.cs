using SportZone.Application.DTOs.Dashboard;

namespace SportZone.Infrastructure.Repositories;

// No extiende Repository<T>: agrega datos de varias entidades, igual que ReporteRepository.
public class DashboardRepository : IDashboardRepository
{
    private readonly ApplicationDbContext _context;

    public DashboardRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> ObtenerParesVendidosAsync(DateTime inicio, DateTime fin)
    {
        return await _context.VentaDetalles
            .Where(d => d.Venta.Estado != "ANULADA" && d.Venta.CreatedAt >= inicio && d.Venta.CreatedAt <= fin)
            .SumAsync(d => d.Cantidad);
    }

    public async Task<List<DashboardVentasPorCategoriaDto>> ObtenerVentasPorCategoriaAsync(DateTime inicio, DateTime fin)
    {
        // Paso 1: agrupar por VarianteId (columna propia, sin navegacion) para que EF Core lo traduzca sin problema
        var porVariante = await _context.VentaDetalles
            .Where(d => d.Venta.Estado != "ANULADA" && d.Venta.CreatedAt >= inicio && d.Venta.CreatedAt <= fin)
            .GroupBy(d => d.VarianteId)
            .Select(g => new { VarianteId = g.Key, Total = g.Sum(d => d.Subtotal) })
            .ToListAsync();

        // Paso 2: resolver la categoria de cada variante aparte (proyeccion simple, sin GroupBy)
        var varianteIds = porVariante.Select(p => p.VarianteId).ToList();
        var categoriaPorVariante = await _context.ArticuloVariantes
            .Where(v => varianteIds.Contains(v.Id))
            .Select(v => new { v.Id, v.Articulo.CategoriaId, v.Articulo.Categoria.Nombre })
            .ToDictionaryAsync(x => x.Id);

        var totalesPorCategoria = new Dictionary<int, (string Nombre, decimal Total)>();
        foreach (var item in porVariante)
        {
            if (!categoriaPorVariante.TryGetValue(item.VarianteId, out var info)) continue;

            var acumulado = totalesPorCategoria.TryGetValue(info.CategoriaId, out var actual)
                ? actual.Total + item.Total
                : item.Total;
            totalesPorCategoria[info.CategoriaId] = (info.Nombre, acumulado);
        }

        var totalGeneral = totalesPorCategoria.Values.Sum(v => v.Total);

        return totalesPorCategoria
            .Select(kv => new DashboardVentasPorCategoriaDto
            {
                CategoriaId = kv.Key,
                CategoriaNombre = kv.Value.Nombre,
                Total = kv.Value.Total,
                Porcentaje = totalGeneral == 0 ? 0 : Math.Round((double)(kv.Value.Total / totalGeneral * 100), 2)
            })
            .OrderByDescending(x => x.Total)
            .ToList();
    }

    public async Task<List<DashboardSerieDiariaDto>> ObtenerSerieDiariaAsync(DateTime inicio, DateTime fin)
    {
        var ventasPorDia = await _context.Ventas
            .Where(v => v.Estado != "ANULADA" && v.CreatedAt >= inicio && v.CreatedAt <= fin)
            .GroupBy(v => v.CreatedAt.Date)
            .Select(g => new { Fecha = g.Key, Total = g.Sum(v => v.Total) })
            .ToListAsync();

        var comprasPorDia = await _context.Ingresos
            .Where(i => i.CreatedAt >= inicio && i.CreatedAt <= fin)
            .GroupBy(i => i.CreatedAt.Date)
            .Select(g => new { Fecha = g.Key, Total = g.Sum(i => i.Total) })
            .ToListAsync();

        var todasLasFechas = ventasPorDia.Select(v => v.Fecha)
            .Union(comprasPorDia.Select(c => c.Fecha))
            .OrderBy(f => f);

        return todasLasFechas.Select(fecha => new DashboardSerieDiariaDto
        {
            Fecha = fecha,
            TotalVentas = ventasPorDia.FirstOrDefault(v => v.Fecha == fecha)?.Total ?? 0,
            TotalCompras = comprasPorDia.FirstOrDefault(c => c.Fecha == fecha)?.Total ?? 0
        }).ToList();
    }
}
