using SportZone.Application.Common.Exceptions;
using SportZone.Application.DTOs.Common;
using SportZone.Application.DTOs.Proveedor;
using SportZone.Application.Interfaces;
using SportZone.Application.Interfaces.Servicios;

namespace SportZone.Application.Servicios;

public class ProveedorServicio : IProveedorServicio
{
    private readonly IProveedorRepository _repository;
    private readonly IRepository<Marca> _marcaRepository;

    public ProveedorServicio(IProveedorRepository repository, IRepository<Marca> marcaRepository)
    {
        _repository = repository;
        _marcaRepository = marcaRepository;
    }

    public async Task<PagedResultDto<ProveedorDto>> GetAllAsync(PaginacionQueryDto query)
    {
        var (items, totalCount) = await _repository.GetPagedWithMarcasAsync(query);

        return new PagedResultDto<ProveedorDto>
        {
            Items = items.Select(MapToDto),
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<ProveedorDto?> GetByIdAsync(int id)
    {
        var proveedor = await _repository.GetByIdWithMarcasAsync(id);
        return proveedor == null ? null : MapToDto(proveedor);
    }

    public async Task<ProveedorDto> CreateAsync(CreateProveedorDto dto)
    {
        if (await _repository.ExisteAsync(p => p.Nombre == dto.Nombre))
            throw new ValidationException($"Ya existe un proveedor con el nombre '{dto.Nombre}'");

        await ValidarMarcasExistenAsync(dto.MarcaIds);

        var proveedor = new Proveedor
        {
            Nombre = dto.Nombre,
            Contacto = dto.Contacto,
            Telefono = dto.Telefono,
            Email = dto.Email,
            Direccion = dto.Direccion,
            CondicionesComerciales = dto.CondicionesComerciales
        };

        var creado = await _repository.CreateAsync(proveedor);
        await _repository.SincronizarMarcasAsync(creado.Id, dto.MarcaIds.Distinct().ToList());

        var creadoConMarcas = await _repository.GetByIdWithMarcasAsync(creado.Id);
        return MapToDto(creadoConMarcas!);
    }

    public async Task<ProveedorDto> UpdateAsync(int id, UpdateProveedorDto dto)
    {
        var proveedor = await _repository.GetByIdAsync(id);
        if (proveedor == null)
            throw new NotFoundException($"Proveedor con Id {id} no encontrado");

        if (await _repository.ExisteAsync(p => p.Nombre == dto.Nombre && p.Id != id))
            throw new ValidationException($"Ya existe otro proveedor con el nombre '{dto.Nombre}'");

        await ValidarMarcasExistenAsync(dto.MarcaIds);

        proveedor.Nombre = dto.Nombre;
        proveedor.Contacto = dto.Contacto;
        proveedor.Telefono = dto.Telefono;
        proveedor.Email = dto.Email;
        proveedor.Direccion = dto.Direccion;
        proveedor.CondicionesComerciales = dto.CondicionesComerciales;

        await _repository.UpdateAsync(proveedor);
        await _repository.SincronizarMarcasAsync(id, dto.MarcaIds.Distinct().ToList());

        var actualizadoConMarcas = await _repository.GetByIdWithMarcasAsync(id);
        return MapToDto(actualizadoConMarcas!);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }

    private async Task ValidarMarcasExistenAsync(List<int> marcaIds)
    {
        foreach (var marcaId in marcaIds.Distinct())
        {
            if (!await _marcaRepository.ExisteAsync(m => m.Id == marcaId))
                throw new ValidationException($"La marca con Id {marcaId} no existe");
        }
    }

    private static ProveedorDto MapToDto(Proveedor proveedor)
    {
        return new ProveedorDto
        {
            Id = proveedor.Id,
            Nombre = proveedor.Nombre,
            Contacto = proveedor.Contacto,
            Telefono = proveedor.Telefono,
            Email = proveedor.Email,
            Direccion = proveedor.Direccion,
            CondicionesComerciales = proveedor.CondicionesComerciales,
            Marcas = proveedor.ProveedorMarcas?
                .Select(pm => new MarcaSuministradaDto { MarcaId = pm.MarcaId, MarcaNombre = pm.Marca?.Nombre ?? string.Empty })
                .ToList() ?? new List<MarcaSuministradaDto>(),
            CreatedAt = proveedor.CreatedAt
        };
    }
}
