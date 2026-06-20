using SportZone.Application.Common.Exceptions;
using SportZone.Application.DTOs.Common;
using SportZone.Application.DTOs.Ingreso;
using SportZone.Application.Interfaces;
using SportZone.Application.Interfaces.Servicios;

namespace SportZone.Application.Servicios;

public class IngresoServicio : IIngresoServicio
{
    private readonly IIngresoRepository _repository;
    private readonly IRepository<Proveedor> _proveedorRepository;
    private readonly IArticuloVarianteRepository _varianteRepository;
    private readonly ICurrentUserService _currentUser;

    public IngresoServicio(
        IIngresoRepository repository,
        IRepository<Proveedor> proveedorRepository,
        IArticuloVarianteRepository varianteRepository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _proveedorRepository = proveedorRepository;
        _varianteRepository = varianteRepository;
        _currentUser = currentUser;
    }

    public async Task<PagedResultDto<IngresoDto>> GetAllAsync(PaginacionQueryDto query)
    {
        var (items, totalCount) = await _repository.GetPagedWithDetalleAsync(query);

        return new PagedResultDto<IngresoDto>
        {
            Items = items.Select(MapToDto),
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<IngresoDto?> GetByIdAsync(int id)
    {
        var ingreso = await _repository.GetByIdWithDetalleAsync(id);
        return ingreso == null ? null : MapToDto(ingreso);
    }

    public async Task<IngresoDto> CreateAsync(CreateIngresoDto dto)
    {
        if (dto.Detalles.Count == 0)
            throw new ValidationException("El ingreso debe tener al menos un detalle");

        if (!await _proveedorRepository.ExisteAsync(p => p.Id == dto.ProveedorId))
            throw new ValidationException($"El proveedor con Id {dto.ProveedorId} no existe");

        var detalles = new List<IngresoDetalle>();
        decimal total = 0;

        foreach (var d in dto.Detalles)
        {
            if (d.Cantidad <= 0)
                throw new ValidationException("La cantidad de cada detalle debe ser mayor a cero");

            if (!await _varianteRepository.ExisteAsync(v => v.Id == d.VarianteId))
                throw new ValidationException($"La variante con Id {d.VarianteId} no existe");

            var subtotal = d.Cantidad * d.PrecioCosto;
            total += subtotal;

            detalles.Add(new IngresoDetalle
            {
                VarianteId = d.VarianteId,
                Cantidad = d.Cantidad,
                PrecioCosto = d.PrecioCosto,
                Subtotal = subtotal
            });
        }

        var ingreso = new Ingreso
        {
            ProveedorId = dto.ProveedorId,
            UsuarioId = _currentUser.GetUsuarioId(),
            NumeroDoc = dto.NumeroDoc,
            FechaDoc = dto.FechaDoc,
            Observacion = dto.Observacion,
            Total = total
        };

        var creado = await _repository.CrearConDetalleAsync(ingreso, detalles);

        var creadoConDetalle = await _repository.GetByIdWithDetalleAsync(creado.Id);
        return MapToDto(creadoConDetalle!);
    }

    private static IngresoDto MapToDto(Ingreso ingreso)
    {
        return new IngresoDto
        {
            Id = ingreso.Id,
            ProveedorId = ingreso.ProveedorId,
            ProveedorNombre = ingreso.Proveedor?.Nombre ?? string.Empty,
            UsuarioId = ingreso.UsuarioId,
            UsuarioNombre = ingreso.Usuario?.Nombre ?? string.Empty,
            NumeroDoc = ingreso.NumeroDoc,
            FechaDoc = ingreso.FechaDoc,
            Total = ingreso.Total,
            Observacion = ingreso.Observacion,
            Detalles = ingreso.IngresoDetalles?.Select(d => new IngresoDetalleDto
            {
                Id = d.Id,
                VarianteId = d.VarianteId,
                VarianteDescripcion = DescribirVariante(d.Variante),
                Cantidad = d.Cantidad,
                PrecioCosto = d.PrecioCosto,
                Subtotal = d.Subtotal
            }).ToList() ?? new List<IngresoDetalleDto>(),
            CreatedAt = ingreso.CreatedAt
        };
    }

    private static string DescribirVariante(ArticuloVariante? variante)
    {
        if (variante == null) return string.Empty;
        var partes = new List<string>();
        if (variante.Articulo != null) partes.Add(variante.Articulo.Nombre);
        if (!string.IsNullOrEmpty(variante.TallaUs)) partes.Add($"Talla {variante.TallaUs}");
        if (!string.IsNullOrEmpty(variante.Color)) partes.Add(variante.Color);
        return string.Join(" - ", partes);
    }
}
