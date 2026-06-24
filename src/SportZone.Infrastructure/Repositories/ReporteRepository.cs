using SportZone.Application.DTOs.Reporte;

namespace SportZone.Infrastructure.Repositories;

// No extiende Repository<T>: un reporte no pertenece a una sola entidad, agrega datos de varias.
public class ReporteRepository : IReporteRepository
{
    private readonly ApplicationDbContext _context;

    public ReporteRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ReporteVentasResumenDto> ObtenerResumenVentasAsync(DateTime? desde, DateTime? hasta)
    {
        var ventas = _context.Ventas.Where(v => v.Estado != "ANULADA");
        if (desde.HasValue) ventas = ventas.Where(v => v.CreatedAt >= desde.Value);
        if (hasta.HasValue) ventas = ventas.Where(v => v.CreatedAt <= hasta.Value);

        var cantidadVentas = await ventas.CountAsync();
        var totalVendido = await ventas.SumAsync(v => v.Total);
        var totalDescuentos = await ventas.SumAsync(v => v.Descuento);

        // El costo vive en el detalle (PrecioCosto se fija al momento de la venta), no en la cabecera
        var detalles = _context.VentaDetalles.Where(d => d.Venta.Estado != "ANULADA");
        if (desde.HasValue) detalles = detalles.Where(d => d.Venta.CreatedAt >= desde.Value);
        if (hasta.HasValue) detalles = detalles.Where(d => d.Venta.CreatedAt <= hasta.Value);

        var totalCosto = await detalles.SumAsync(d => d.PrecioCosto * d.Cantidad);
        var gananciaBruta = totalVendido - totalCosto;
        var ticketPromedio = cantidadVentas == 0 ? 0 : totalVendido / cantidadVentas;

        return new ReporteVentasResumenDto
        {
            Desde = desde,
            Hasta = hasta,
            CantidadVentas = cantidadVentas,
            TotalVendido = totalVendido,
            TotalDescuentos = totalDescuentos,
            TotalCosto = totalCosto,
            GananciaBruta = gananciaBruta,
            TicketPromedio = ticketPromedio
        };
    }

    public async Task<List<ReporteArticuloVendidoDto>> ObtenerTopArticulosVendidosAsync(DateTime? desde, DateTime? hasta, int top)
    {
        var detalles = _context.VentaDetalles.Where(d => d.Venta.Estado != "ANULADA");
        if (desde.HasValue) detalles = detalles.Where(d => d.Venta.CreatedAt >= desde.Value);
        if (hasta.HasValue) detalles = detalles.Where(d => d.Venta.CreatedAt <= hasta.Value);

        var agrupado = await detalles
            .GroupBy(d => d.VarianteId)
            .Select(g => new
            {
                VarianteId = g.Key,
                CantidadVendida = g.Sum(d => d.Cantidad),
                TotalVendido = g.Sum(d => d.Subtotal),
                TotalCosto = g.Sum(d => d.PrecioCosto * d.Cantidad)
            })
            .OrderByDescending(x => x.CantidadVendida)
            .Take(top)
            .ToListAsync();

        // Las variantes se traen aparte (no mezclar GroupBy con navegaciones profundas en la misma consulta)
        var varianteIds = agrupado.Select(a => a.VarianteId).ToList();
        var variantes = await _context.ArticuloVariantes
            .Include(v => v.Articulo)
            .Where(v => varianteIds.Contains(v.Id))
            .ToDictionaryAsync(v => v.Id);

        return agrupado.Select(a =>
        {
            variantes.TryGetValue(a.VarianteId, out var variante);
            return new ReporteArticuloVendidoDto
            {
                VarianteId = a.VarianteId,
                ArticuloId = variante?.ArticuloId ?? 0,
                ArticuloNombre = variante?.Articulo?.Nombre ?? string.Empty,
                VarianteDescripcion = DescribirVariante(variante),
                CantidadVendida = a.CantidadVendida,
                TotalVendido = a.TotalVendido,
                GananciaGenerada = a.TotalVendido - a.TotalCosto
            };
        }).ToList();
    }

    public async Task<ReporteComprasResumenDto> ObtenerResumenComprasAsync(DateTime? desde, DateTime? hasta)
    {
        var ingresos = _context.Ingresos.AsQueryable();
        if (desde.HasValue) ingresos = ingresos.Where(i => i.CreatedAt >= desde.Value);
        if (hasta.HasValue) ingresos = ingresos.Where(i => i.CreatedAt <= hasta.Value);

        var cantidadIngresos = await ingresos.CountAsync();
        var totalComprado = await ingresos.SumAsync(i => i.Total);
        var compraPromedio = cantidadIngresos == 0 ? 0 : totalComprado / cantidadIngresos;

        return new ReporteComprasResumenDto
        {
            Desde = desde,
            Hasta = hasta,
            CantidadIngresos = cantidadIngresos,
            TotalComprado = totalComprado,
            CompraPromedio = compraPromedio
        };
    }

    public async Task<List<ReporteComprasPorProveedorDto>> ObtenerComprasPorProveedorAsync(DateTime? desde, DateTime? hasta)
    {
        var ingresos = _context.Ingresos.AsQueryable();
        if (desde.HasValue) ingresos = ingresos.Where(i => i.CreatedAt >= desde.Value);
        if (hasta.HasValue) ingresos = ingresos.Where(i => i.CreatedAt <= hasta.Value);

        var agrupado = await ingresos
            .GroupBy(i => i.ProveedorId)
            .Select(g => new
            {
                ProveedorId = g.Key,
                CantidadIngresos = g.Count(),
                TotalComprado = g.Sum(i => i.Total)
            })
            .OrderByDescending(x => x.TotalComprado)
            .ToListAsync();

        var proveedorIds = agrupado.Select(a => a.ProveedorId).ToList();
        var proveedores = await _context.Proveedores
            .Where(p => proveedorIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        return agrupado.Select(a =>
        {
            proveedores.TryGetValue(a.ProveedorId, out var proveedor);
            return new ReporteComprasPorProveedorDto
            {
                ProveedorId = a.ProveedorId,
                ProveedorNombre = proveedor?.Nombre ?? string.Empty,
                CantidadIngresos = a.CantidadIngresos,
                TotalComprado = a.TotalComprado
            };
        }).ToList();
    }

    public async Task<ReporteVentasDetalladoDto> ObtenerReporteVentasAsync(DateTime? desde, DateTime? hasta)
    {
        var resumen = await ObtenerResumenVentasAsync(desde, hasta);

        // El listado muestra TODAS las ventas del período (incluye anuladas, vía Estado) para que sea
        // una bitácora completa; el resumen arriba es el que excluye anuladas en sus totales financieros.
        var ventas = _context.Ventas
            .Include(v => v.Cliente)
            .Include(v => v.VentaDetalles).ThenInclude(d => d.Variante).ThenInclude(va => va.Articulo)
            .AsQueryable();
        if (desde.HasValue) ventas = ventas.Where(v => v.CreatedAt >= desde.Value);
        if (hasta.HasValue) ventas = ventas.Where(v => v.CreatedAt <= hasta.Value);

        var lista = await ventas.OrderByDescending(v => v.CreatedAt).ToListAsync();

        return new ReporteVentasDetalladoDto
        {
            Resumen = resumen,
            Ventas = lista.Select(v =>
            {
                var costo = v.VentaDetalles.Sum(d => d.PrecioCosto * d.Cantidad);
                return new ReporteVentaItemDto
                {
                    Id = v.Id,
                    NumeroDoc = v.NumeroDoc,
                    TipoComprobante = v.TipoComprobante,
                    ClienteNombre = v.Cliente?.Nombre,
                    Estado = v.Estado,
                    Total = v.Total,
                    Descuento = v.Descuento,
                    Costo = costo,
                    Ganancia = v.Total - costo,
                    CreatedAt = v.CreatedAt,
                    Detalles = v.VentaDetalles.Select(d => new ReporteVentaDetalleItemDto
                    {
                        Nombre = DescribirVarianteCompleta(d.Variante),
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario,
                        Total = d.Subtotal
                    }).ToList()
                };
            }).ToList()
        };
    }

    public async Task<ReporteComprasDetalladoDto> ObtenerReporteComprasAsync(DateTime? desde, DateTime? hasta)
    {
        var resumen = await ObtenerResumenComprasAsync(desde, hasta);

        var ingresos = _context.Ingresos
            .Include(i => i.Proveedor)
            .Include(i => i.IngresoDetalles).ThenInclude(d => d.Variante).ThenInclude(v => v.Articulo)
            .AsQueryable();
        if (desde.HasValue) ingresos = ingresos.Where(i => i.CreatedAt >= desde.Value);
        if (hasta.HasValue) ingresos = ingresos.Where(i => i.CreatedAt <= hasta.Value);

        var lista = await ingresos.OrderByDescending(i => i.CreatedAt).ToListAsync();

        return new ReporteComprasDetalladoDto
        {
            Resumen = resumen,
            Compras = lista.Select(i => new ReporteCompraItemDto
            {
                Id = i.Id,
                NumeroDoc = i.NumeroDoc,
                ProveedorNombre = i.Proveedor?.Nombre ?? string.Empty,
                Total = i.Total,
                CreatedAt = i.CreatedAt,
                Detalles = i.IngresoDetalles.Select(d => new ReporteCompraDetalleItemDto
                {
                    Nombre = DescribirVarianteCompleta(d.Variante),
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioCosto,
                    Total = d.Subtotal
                }).ToList()
            }).ToList()
        };
    }

    private static string DescribirVariante(ArticuloVariante? variante)
    {
        if (variante == null) return string.Empty;
        var partes = new List<string>();
        if (!string.IsNullOrEmpty(variante.TallaUs)) partes.Add($"Talla {variante.TallaUs}");
        if (!string.IsNullOrEmpty(variante.Color)) partes.Add(variante.Color);
        return string.Join(" - ", partes);
    }

    // A diferencia de DescribirVariante, esta sí incluye el nombre del articulo: se usa en listados
    // de detalle (ventas/compras) donde no hay un campo ArticuloNombre separado, solo "Nombre".
    private static string DescribirVarianteCompleta(ArticuloVariante? variante)
    {
        if (variante == null) return string.Empty;
        var partes = new List<string>();
        if (variante.Articulo != null) partes.Add(variante.Articulo.Nombre);
        if (!string.IsNullOrEmpty(variante.TallaUs)) partes.Add($"Talla {variante.TallaUs}");
        if (!string.IsNullOrEmpty(variante.Color)) partes.Add(variante.Color);
        return string.Join(" - ", partes);
    }
}
