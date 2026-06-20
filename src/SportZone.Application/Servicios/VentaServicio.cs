using SportZone.Application.Common.Exceptions;
using SportZone.Application.DTOs.Common;
using SportZone.Application.DTOs.Venta;
using SportZone.Application.Interfaces;
using SportZone.Application.Interfaces.Servicios;

namespace SportZone.Application.Servicios;

public class VentaServicio : IVentaServicio
{
    private readonly IVentaRepository _repository;
    private readonly IArticuloVarianteRepository _varianteRepository;
    private readonly IRepository<Cliente> _clienteRepository;

    public VentaServicio(
        IVentaRepository repository,
        IArticuloVarianteRepository varianteRepository,
        IRepository<Cliente> clienteRepository)
    {
        _repository = repository;
        _varianteRepository = varianteRepository;
        _clienteRepository = clienteRepository;
    }

    public async Task<PagedResultDto<VentaDto>> GetAllAsync(PaginacionQueryDto query)
    {
        var (items, totalCount) = await _repository.GetPagedWithDetalleAsync(query);

        return new PagedResultDto<VentaDto>
        {
            Items = items.Select(MapToDto),
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<VentaDto?> GetByIdAsync(int id)
    {
        var venta = await _repository.GetByIdWithDetalleAsync(id);
        return venta == null ? null : MapToDto(venta);
    }

    public async Task<VentaDto> CreateAsync(CreateVentaDto dto)
    {
        if (dto.Detalles.Count == 0)
            throw new ValidationException("La venta debe tener al menos un detalle");

        if (dto.ClienteId.HasValue && !await _clienteRepository.ExisteAsync(c => c.Id == dto.ClienteId.Value))
            throw new ValidationException($"El cliente con Id {dto.ClienteId} no existe");

        var detalles = new List<VentaDetalle>();
        decimal subtotal = 0;
        decimal descuentoTotal = 0;

        // El precio se calcula aquí, desde la Variante real en base de datos — nunca se confía
        // en un precio enviado por el cliente, y se valida que haya stock suficiente.
        foreach (var d in dto.Detalles)
        {
            if (d.Cantidad <= 0)
                throw new ValidationException("La cantidad de cada detalle debe ser mayor a cero");

            var variante = await _varianteRepository.GetByIdWithArticuloAsync(d.VarianteId);
            if (variante == null)
                throw new ValidationException($"La variante con Id {d.VarianteId} no existe");

            if (variante.Stock < d.Cantidad)
                throw new ValidationException(
                    $"Stock insuficiente para la variante Id {d.VarianteId}. Disponible: {variante.Stock}, solicitado: {d.Cantidad}");

            var precioUnitario = variante.PrecioVenta;
            var subtotalLinea = (d.Cantidad * precioUnitario) - d.Descuento;

            subtotal += d.Cantidad * precioUnitario;
            descuentoTotal += d.Descuento;

            detalles.Add(new VentaDetalle
            {
                VarianteId = d.VarianteId,
                Cantidad = d.Cantidad,
                PrecioUnitario = precioUnitario,
                Descuento = d.Descuento,
                Subtotal = subtotalLinea
            });
        }

        var total = subtotal - descuentoTotal;

        var prefijo = dto.TipoComprobante == "FACTURA" ? "FAC" : "REC";
        var numeroDoc = $"{prefijo}-{DateTime.UtcNow:yyyyMMddHHmmssfff}";

        var venta = new Venta
        {
            ClienteId = dto.ClienteId,
            NumeroDoc = numeroDoc,
            TipoComprobante = dto.TipoComprobante,
            Subtotal = subtotal,
            Descuento = descuentoTotal,
            Total = total,
            Estado = "PAGADA",
            Observacion = dto.Observacion
        };

        var creada = await _repository.CrearConDetalleAsync(venta, detalles);

        var creadaConDetalle = await _repository.GetByIdWithDetalleAsync(creada.Id);
        return MapToDto(creadaConDetalle!);
    }

    public async Task<VentaDto> AnularAsync(int id)
    {
        var venta = await _repository.GetByIdWithDetalleAsync(id);
        if (venta == null)
            throw new NotFoundException($"Venta con Id {id} no encontrada");

        if (venta.Estado == "ANULADA")
            throw new ValidationException("La venta ya esta anulada");

        await _repository.AnularConReintegroAsync(venta);

        var actualizada = await _repository.GetByIdWithDetalleAsync(id);
        return MapToDto(actualizada!);
    }

    private static VentaDto MapToDto(Venta venta)
    {
        return new VentaDto
        {
            Id = venta.Id,
            ClienteId = venta.ClienteId,
            ClienteNombre = venta.Cliente?.Nombre,
            NumeroDoc = venta.NumeroDoc,
            TipoComprobante = venta.TipoComprobante,
            Subtotal = venta.Subtotal,
            Descuento = venta.Descuento,
            Total = venta.Total,
            Estado = venta.Estado,
            Observacion = venta.Observacion,
            Detalles = venta.VentaDetalles?.Select(d => new VentaDetalleDto
            {
                Id = d.Id,
                VarianteId = d.VarianteId,
                VarianteDescripcion = DescribirVariante(d.Variante),
                Cantidad = d.Cantidad,
                PrecioUnitario = d.PrecioUnitario,
                Descuento = d.Descuento,
                Subtotal = d.Subtotal
            }).ToList() ?? new List<VentaDetalleDto>(),
            CreatedAt = venta.CreatedAt
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
