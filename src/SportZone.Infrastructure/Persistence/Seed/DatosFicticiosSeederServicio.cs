using SportZone.Application.Interfaces.Servicios;

namespace SportZone.Infrastructure.Persistence.Seed;

// Genera Proveedores, Clientes, Ingresos y Ventas ficticios desde el 20 de mayo hasta la fecha actual,
// para que el catalogo (cargado en Stock 0 por DataSeederServicio) tenga movimiento real que mostrar
// en Reportes/Dashboard. Usa semilla fija para que los datos sean reproducibles tras cada reset de BD.
public class DatosFicticiosSeederServicio : IDatosFicticiosSeederServicio
{
    private static readonly DateTime FechaInicio = new(2026, 5, 20);

    // Categoria.Id -> peso relativo de aparicion en las ventas (proporcional al tamano real del catalogo)
    private static readonly Dictionary<int, int> PesoCategoria = new() { [3] = 60, [1] = 20, [2] = 12, [7] = 8 };

    private readonly ApplicationDbContext _context;
    private readonly Random _random = new(20260520);
    private int _contadorDocumento;

    public DatosFicticiosSeederServicio(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        if (await _context.Ventas.AnyAsync() || await _context.Ingresos.AnyAsync())
            return;

        var variantes = await _context.ArticuloVariantes.Include(v => v.Articulo).ToListAsync();
        if (variantes.Count == 0)
            return; // el catalogo todavia no se cargo, no hay nada que vender/comprar

        var adminId = (await _context.Usuarios.OrderBy(u => u.Id).FirstAsync()).Id;
        var fechaHasta = DateTime.UtcNow.Date;

        var proveedores = CrearProveedores(adminId);
        var clientes = CrearClientes(adminId);
        var estado = variantes.ToDictionary(v => v.Id, v => new EstadoVariante(v));

        GenerarStockInicial(variantes, estado, proveedores, adminId);

        var ventasGeneradas = new List<Venta>();
        for (var fecha = FechaInicio; fecha <= fechaHasta; fecha = fecha.AddDays(1))
        {
            ventasGeneradas.AddRange(GenerarVentasDelDia(fecha, variantes, estado, clientes, adminId));
            ReponerSiHaceFalta(fecha, variantes, estado, proveedores, adminId);
        }

        AnularAlgunasVentas(ventasGeneradas, estado, fechaHasta, adminId);

        // Refleja en las entidades reales (ya trackeadas por el Include de arriba) el resultado final de la simulacion
        foreach (var variante in variantes)
        {
            var e = estado[variante.Id];
            variante.Stock = e.Stock;
            variante.PrecioCosto = e.PrecioCosto;
        }

        // SaveChanges SINCRONO (no el SaveChangesAsync sobreescrito) para que NO se pisen los CreatedAt
        // explicitos con DateTime.UtcNow -- mismo truco que usa DataSeederServicio para el catalogo.
        _context.SaveChanges();
    }

    private List<Proveedor> CrearProveedores(int adminId)
    {
        var proveedores = new List<Proveedor>
        {
            new() { Nombre = "Distribuidora Deportiva SRL", Contacto = "Marco Quispe", Telefono = "70011223", Email = "ventas@distdeportiva.bo", Direccion = "Av. Blanco Galindo km 5, Cochabamba" },
            new() { Nombre = "Import Sport Bolivia", Contacto = "Lucia Fernandez", Telefono = "70022334", Email = "contacto@importsport.bo", Direccion = "C. Comercio 450, La Paz" },
            new() { Nombre = "Calzados El Pacifico", Contacto = "Ramiro Vargas", Telefono = "70033445", Email = "pedidos@calzadospacifico.bo", Direccion = "Av. Cristo Redentor 220, Santa Cruz" },
            new() { Nombre = "Mayorista Andino Sport", Contacto = "Patricia Choque", Telefono = "70044556", Email = "info@andinosport.bo", Direccion = "Av. Heroinas 110, Cochabamba" }
        };

        foreach (var proveedor in proveedores)
        {
            proveedor.CreatedAt = FechaInicio;
            proveedor.CreateById = adminId;
        }

        _context.Proveedores.AddRange(proveedores);
        return proveedores;
    }

    private List<Cliente> CrearClientes(int adminId)
    {
        var datos = new[]
        {
            ("Juan Mamani", "4123456"), ("Maria Quispe", "5234567"), ("Carlos Choque", "6345678"),
            ("Ana Flores", "4456789"), ("Pedro Condori", "5567890"), ("Lucia Vargas", "6678901"),
            ("Jorge Ticona", "4789012"), ("Rosa Apaza", "5890123"), ("Miguel Calle", "6901234"),
            ("Daniela Rojas", "4012345")
        };

        var clientes = datos.Select(d => new Cliente
        {
            Nombre = d.Item1,
            TipoDocumento = "CI",
            Documento = d.Item2,
            Telefono = $"7{_random.Next(1000000, 9999999)}",
            CreatedAt = FechaInicio,
            CreateById = adminId
        }).ToList();

        _context.Clientes.AddRange(clientes);
        return clientes;
    }

    // Reparte las 181 variantes en grupos de ~3 articulos por ingreso, rotando proveedor,
    // todos fechados entre el 20 y el 22 de mayo: asi el catalogo no arranca con stock 0.
    private void GenerarStockInicial(
        List<ArticuloVariante> variantes, Dictionary<int, EstadoVariante> estado, List<Proveedor> proveedores, int adminId)
    {
        var gruposPorArticulo = variantes.GroupBy(v => v.ArticuloId).Chunk(3);

        foreach (var grupo in gruposPorArticulo)
        {
            var proveedor = proveedores[_random.Next(proveedores.Count)];
            var fecha = FechaInicio.AddDays(_random.Next(0, 3)).AddHours(9 + _random.Next(0, 8));
            var detalles = new List<IngresoDetalle>();
            decimal total = 0;

            foreach (var variantesDelArticulo in grupo)
            {
                foreach (var variante in variantesDelArticulo)
                {
                    var cantidad = _random.Next(15, 36);
                    var costo = estado[variante.Id].PrecioCosto;
                    var subtotal = cantidad * costo;
                    total += subtotal;

                    detalles.Add(new IngresoDetalle
                    {
                        VarianteId = variante.Id,
                        Cantidad = cantidad,
                        PrecioCosto = costo,
                        Subtotal = subtotal,
                        CreatedAt = fecha,
                        CreateById = adminId
                    });

                    estado[variante.Id].Stock += cantidad;
                }
            }

            AgregarIngreso(proveedor, detalles, fecha, adminId, total);
        }
    }

    private List<Venta> GenerarVentasDelDia(
        DateTime fecha, List<ArticuloVariante> variantes, Dictionary<int, EstadoVariante> estado, List<Cliente> clientes, int adminId)
    {
        var ventasDelDia = new List<Venta>();
        var esDomingo = fecha.DayOfWeek == DayOfWeek.Sunday;
        var cantidadVentas = esDomingo ? _random.Next(0, 3) : _random.Next(1, 6);

        for (var i = 0; i < cantidadVentas; i++)
        {
            var venta = GenerarUnaVenta(fecha, variantes, estado, clientes, adminId);
            if (venta == null) continue;

            _context.Ventas.Add(venta);
            ventasDelDia.Add(venta);
        }

        return ventasDelDia;
    }

    private Venta? GenerarUnaVenta(
        DateTime fecha, List<ArticuloVariante> variantes, Dictionary<int, EstadoVariante> estado, List<Cliente> clientes, int adminId)
    {
        var lineasDeseadas = _random.Next(1, 4);
        var detalles = new List<VentaDetalle>();
        var varianteIdsUsadas = new HashSet<int>();
        decimal subtotal = 0;
        decimal descuentoTotal = 0;
        var horaVenta = HoraDeNegocio(fecha);

        for (var l = 0; l < lineasDeseadas; l++)
        {
            var candidatas = variantes.Where(v => !varianteIdsUsadas.Contains(v.Id)).ToList();
            var variante = ElegirVarianteConStock(candidatas, estado);
            if (variante == null) break;

            var disponible = estado[variante.Id].Stock;
            var cantidad = Math.Min(disponible, _random.Next(1, 3));
            if (cantidad <= 0) continue;

            varianteIdsUsadas.Add(variante.Id);
            var precioUnitario = estado[variante.Id].PrecioVenta;
            var descuentoLinea = _random.NextDouble() < 0.15 ? Math.Round(precioUnitario * cantidad * 0.1m, 2) : 0m;
            var subtotalLinea = (cantidad * precioUnitario) - descuentoLinea;

            subtotal += cantidad * precioUnitario;
            descuentoTotal += descuentoLinea;

            detalles.Add(new VentaDetalle
            {
                VarianteId = variante.Id,
                Cantidad = cantidad,
                PrecioUnitario = precioUnitario,
                PrecioCosto = estado[variante.Id].PrecioCosto,
                Descuento = descuentoLinea,
                Subtotal = subtotalLinea,
                CreatedAt = horaVenta,
                CreateById = adminId
            });

            estado[variante.Id].Stock -= cantidad;
        }

        if (detalles.Count == 0)
            return null;

        var cliente = _random.NextDouble() < 0.7 ? clientes[_random.Next(clientes.Count)] : null;
        var tipoComprobante = _random.NextDouble() < 0.2 ? "FACTURA" : "RECIBO";
        var prefijo = tipoComprobante == "FACTURA" ? "FAC" : "REC";

        var venta = new Venta
        {
            Cliente = cliente,
            NumeroDoc = $"{prefijo}-{horaVenta:yyyyMMddHHmmss}-{++_contadorDocumento:0000}",
            TipoComprobante = tipoComprobante,
            Subtotal = subtotal,
            Descuento = descuentoTotal,
            Total = subtotal - descuentoTotal,
            Estado = "PAGADA",
            CreatedAt = horaVenta,
            CreateById = adminId
        };

        foreach (var detalle in detalles)
        {
            venta.VentaDetalles.Add(detalle);
            venta.MovimientosStock.Add(new MovimientoStock
            {
                ArticuloVarianteId = detalle.VarianteId,
                TipoMovimiento = "SALIDA",
                Cantidad = detalle.Cantidad,
                NumeroDoc = venta.NumeroDoc,
                CreatedAt = horaVenta,
                CreateById = adminId
            });
        }

        return venta;
    }

    // Repone los lunes y jueves cualquier variante por debajo del doble de su stock minimo.
    private void ReponerSiHaceFalta(
        DateTime fecha, List<ArticuloVariante> variantes, Dictionary<int, EstadoVariante> estado, List<Proveedor> proveedores, int adminId)
    {
        if (fecha.DayOfWeek != DayOfWeek.Monday && fecha.DayOfWeek != DayOfWeek.Thursday)
            return;

        // Piso minimo de 12 (no solo StockMinimo*2): el stock inicial es de 15-35 u. y StockMinimo
        // suele ser 2, asi que un umbral tan bajo casi nunca se alcanza y todas las compras quedarian
        // concentradas en el stock inicial de mayo, sin reposicion real en junio.
        var bajos = variantes.Where(v => estado[v.Id].Stock < Math.Max(estado[v.Id].StockMinimo * 2, 12)).Take(15).ToList();
        if (bajos.Count == 0)
            return;

        var proveedor = proveedores[_random.Next(proveedores.Count)];
        var horaIngreso = HoraDeNegocio(fecha);
        var detalles = new List<IngresoDetalle>();
        decimal total = 0;

        foreach (var variante in bajos)
        {
            var cantidad = _random.Next(20, 41);
            var costoNuevo = AjustarCostoLigeramente(estado[variante.Id].PrecioCosto);
            var subtotal = cantidad * costoNuevo;
            total += subtotal;

            detalles.Add(new IngresoDetalle
            {
                VarianteId = variante.Id,
                Cantidad = cantidad,
                PrecioCosto = costoNuevo,
                Subtotal = subtotal,
                CreatedAt = horaIngreso,
                CreateById = adminId
            });

            estado[variante.Id].Stock += cantidad;
            estado[variante.Id].PrecioCosto = costoNuevo; // el ultimo costo de compra queda como costo vigente
        }

        AgregarIngreso(proveedor, detalles, horaIngreso, adminId, total);
    }

    private void AgregarIngreso(Proveedor proveedor, List<IngresoDetalle> detalles, DateTime fecha, int adminId, decimal total)
    {
        var ingreso = new Ingreso
        {
            Proveedor = proveedor,
            NumeroDoc = $"FAC-{fecha:yyyyMMddHHmmss}-{++_contadorDocumento:0000}",
            Total = total,
            CreatedAt = fecha,
            CreateById = adminId
        };

        foreach (var detalle in detalles)
        {
            ingreso.IngresoDetalles.Add(detalle);
            ingreso.MovimientosStock.Add(new MovimientoStock
            {
                ArticuloVarianteId = detalle.VarianteId,
                TipoMovimiento = "ENTRADA",
                Cantidad = detalle.Cantidad,
                NumeroDoc = ingreso.NumeroDoc,
                CreatedAt = fecha,
                CreateById = adminId
            });
        }

        _context.Ingresos.Add(ingreso);
    }

    // Simplificacion consciente: el reintegro se aplica al stock FINAL de la variante (no se vuelve a
    // recorrer la linea de tiempo), suficiente para datos de demostracion. Nunca anula la venta del ultimo dia.
    private void AnularAlgunasVentas(List<Venta> ventas, Dictionary<int, EstadoVariante> estado, DateTime fechaHasta, int adminId)
    {
        var candidatas = ventas.Where(v => v.CreatedAt.Date < fechaHasta).ToList();
        var cantidadAAnular = (int)Math.Round(candidatas.Count * 0.05);

        foreach (var venta in candidatas.OrderBy(_ => _random.Next()).Take(cantidadAAnular))
        {
            venta.Estado = "ANULADA";
            var fechaAnulacion = venta.CreatedAt.AddDays(1) > fechaHasta ? fechaHasta : venta.CreatedAt.AddDays(1);

            foreach (var detalle in venta.VentaDetalles)
            {
                estado[detalle.VarianteId].Stock += detalle.Cantidad;
                venta.MovimientosStock.Add(new MovimientoStock
                {
                    ArticuloVarianteId = detalle.VarianteId,
                    TipoMovimiento = "ENTRADA",
                    Cantidad = detalle.Cantidad,
                    NumeroDoc = venta.NumeroDoc,
                    CreatedAt = fechaAnulacion,
                    CreateById = adminId
                });
            }
        }
    }

    private ArticuloVariante? ElegirVarianteConStock(List<ArticuloVariante> candidatas, Dictionary<int, EstadoVariante> estado)
    {
        var disponibles = candidatas.Where(v => estado[v.Id].Stock > 0).ToList();
        if (disponibles.Count == 0)
            return null;

        var totalPeso = disponibles.Sum(v => PesoCategoria.GetValueOrDefault(estado[v.Id].CategoriaId, 5));
        var roll = _random.Next(0, totalPeso);
        var acumulado = 0;

        foreach (var variante in disponibles)
        {
            acumulado += PesoCategoria.GetValueOrDefault(estado[variante.Id].CategoriaId, 5);
            if (roll < acumulado)
                return variante;
        }

        return disponibles[^1];
    }

    private DateTime HoraDeNegocio(DateTime fecha) => fecha.Date.AddHours(9).AddMinutes(_random.Next(0, 11 * 60));

    private decimal AjustarCostoLigeramente(decimal costoActual)
    {
        var variacion = 1 + ((_random.NextDouble() * 0.06) - 0.03); // entre -3% y +3%
        return Math.Round(costoActual * (decimal)variacion, 2);
    }

    private sealed class EstadoVariante
    {
        public int Stock { get; set; }
        public decimal PrecioCosto { get; set; }
        public decimal PrecioVenta { get; }
        public int StockMinimo { get; }
        public int CategoriaId { get; }

        public EstadoVariante(ArticuloVariante variante)
        {
            Stock = variante.Stock;
            PrecioCosto = variante.PrecioCosto;
            PrecioVenta = variante.PrecioVenta;
            StockMinimo = variante.StockMinimo;
            CategoriaId = variante.Articulo.CategoriaId;
        }
    }
}
