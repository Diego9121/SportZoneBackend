using System.Text.Json;
using SportZone.Application.Common.Exceptions;
using SportZone.Application.Interfaces.Servicios;

namespace SportZone.Infrastructure.Persistence.Seed;

public class DataSeederServicio : IDataSeederServicio
{
    private readonly ApplicationDbContext _context;

    public DataSeederServicio(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> CargarDesdeJsonAsync(string rutaArchivoJson)
    {
        if (!File.Exists(rutaArchivoJson))
            throw new ValidationException($"No se encontro el archivo de seed en '{rutaArchivoJson}'");

        // Evita duplicar todo si el seeder ya se ejecutó antes
        if (await _context.Marcas.AnyAsync())
            throw new ValidationException("Ya existen Marcas en la base de datos; el seeder no se vuelve a ejecutar para evitar duplicados");

        var json = await File.ReadAllTextAsync(rutaArchivoJson);
        var opciones = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };

        var datos = JsonSerializer.Deserialize<SeederDataDto>(json, opciones)
            ?? throw new ValidationException("El archivo de seed esta vacio o mal formado");

        // Orden importante: Marca/Categoria primero (Articulo las referencia), luego Articulo,
        // y al final Variante (que referencia Articulo). Si se invierte, fallan las FK.
        await InsertarConIdentityInsertAsync("Marcas", () =>
            _context.Marcas.AddRange(datos.Marcas.Select(m => new Marca
            {
                Id = m.Id,
                Nombre = m.Nombre,
                Descripcion = m.Descripcion,
                Logo = m.Logo,
                CreateById = m.CreateById,
                CreatedAt = m.CreatedAt
            })));

        await InsertarConIdentityInsertAsync("Categorias", () =>
            _context.Categorias.AddRange(datos.Categorias.Select(c => new Categoria
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Descripcion = c.Descripcion,
                CreateById = c.CreateById,
                CreatedAt = c.CreatedAt
            })));

        await InsertarConIdentityInsertAsync("Articulos", () =>
            _context.Articulos.AddRange(datos.Articulos.Select(a => new Articulo
            {
                Id = a.Id,
                CategoriaId = a.CategoriaId,
                MarcaId = a.MarcaId,
                Codigo = a.Codigo,
                Nombre = a.Nombre,
                Descripcion = a.Descripcion,
                Imagen = a.Imagen,
                CreateById = a.CreateById,
                CreatedAt = a.CreatedAt
            })));

        await InsertarConIdentityInsertAsync("ArticuloVariantes", () =>
            _context.ArticuloVariantes.AddRange(datos.ArticuloVariantes.Select(v => new ArticuloVariante
            {
                Id = v.Id,
                ArticuloId = v.ArticuloId,
                TallaUs = v.TallaUs,
                TallaEu = v.TallaEu,
                TallaUk = v.TallaUk,
                TallaCm = v.TallaCm,
                Color = v.Color,
                CodigoBarras = v.CodigoBarras,
                Stock = v.Stock,
                StockMinimo = v.StockMinimo,
                PrecioVenta = v.PrecioVenta,
                PrecioCosto = v.PrecioCosto,
                CreateById = v.CreateById,
                CreatedAt = v.CreatedAt
            })));

        return $"Cargados: {datos.Marcas.Count} marcas, {datos.Categorias.Count} categorias, " +
               $"{datos.Articulos.Count} articulos, {datos.ArticuloVariantes.Count} variantes";
    }

    // SET IDENTITY_INSERT permite insertar respetando el Id exacto del JSON en vez de que SQL Server
    // genere uno nuevo (necesario porque articulo_variantes referencia articulos por ese mismo Id).
    // Se usa una transacción explícita para garantizar que el ON/OFF y el INSERT comparten la misma conexión,
    // y SaveChanges() SINCRONO (no el SaveChangesAsync sobreescrito) para que NO se pisen CreatedAt/CreateById
    // con los valores automáticos — se respetan los del archivo de seed.
    private async Task InsertarConIdentityInsertAsync(string tabla, Action agregarEntidades)
    {
        await using var transaccion = await _context.Database.BeginTransactionAsync();

        // "tabla" siempre llega de un literal fijo escrito en este mismo archivo (nunca de afuera/usuario),
        // por eso es seguro pese a la advertencia genérica de EF1002 sobre cadenas interpoladas en SQL.
#pragma warning disable EF1002
        await _context.Database.ExecuteSqlRawAsync($"SET IDENTITY_INSERT [{tabla}] ON");
        agregarEntidades();
        _context.SaveChanges();
        await _context.Database.ExecuteSqlRawAsync($"SET IDENTITY_INSERT [{tabla}] OFF");
#pragma warning restore EF1002

        await transaccion.CommitAsync();
    }
}
