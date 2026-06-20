using SportZone.Application.Common.Exceptions;
using SportZone.Application.DTOs.Common;
using SportZone.Application.DTOs.Usuario;
using SportZone.Application.Interfaces;
using SportZone.Application.Interfaces.Servicios;

namespace SportZone.Application.Servicios;

// Orquesta el CRUD de Usuario. CreateById/UpdateById/DeleteById los asigna el Repository automáticamente.
public class UsuarioServicio : IUsuarioServicio
{
    private readonly IUsuarioRepository _repository;

    public UsuarioServicio(IUsuarioRepository repository)
    {
        _repository = repository;
    }

    // Lista paginada; cada Usuario ya viene con su Rol cargado (Include) para poder mostrar RolNombre
    public async Task<PagedResultDto<UsuarioDto>> GetAllAsync(PaginacionQueryDto query)
    {
        var (items, totalCount) = await _repository.GetPagedWithRolAsync(query);

        return new PagedResultDto<UsuarioDto>
        {
            Items = items.Select(MapToDto),
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<UsuarioDto?> GetByIdAsync(int id)
    {
        var usuario = await _repository.GetByIdWithRolAsync(id);
        return usuario == null ? null : MapToDto(usuario);
    }

    // Valida email único, hashea el password y crea. El Rol no se valida aquí todavía
    // (se podría agregar verificando que RolId exista, pendiente como mejora futura).
    public async Task<UsuarioDto> CreateAsync(CreateUsuarioDto dto)
    {
        if (await _repository.ExisteEmailAsync(dto.Email))
            throw new ValidationException($"Ya existe un usuario con el email '{dto.Email}'");

        var usuario = new Usuario
        {
            Nombre = dto.Nombre,
            Email = dto.Email,
            PasswordHash = HashPassword(dto.Password),
            RolId = dto.RolId
        };

        var creado = await _repository.CreateAsync(usuario);

        // Se vuelve a pedir con el Rol incluido para poder devolver RolNombre en la respuesta
        var creadoConRol = await _repository.GetByIdWithRolAsync(creado.Id);
        return MapToDto(creadoConRol!);
    }

    public async Task<UsuarioDto> UpdateAsync(int id, UpdateUsuarioDto dto)
    {
        var usuario = await _repository.GetByIdAsync(id);
        if (usuario == null)
            throw new NotFoundException($"Usuario con Id {id} no encontrado");

        usuario.Nombre = dto.Nombre;
        usuario.Email = dto.Email;
        usuario.RolId = dto.RolId;
        usuario.Activo = dto.Activo; // posible gracias a IActivable, aunque aquí se usa directo por simplicidad

        await _repository.UpdateAsync(usuario);

        var actualizadoConRol = await _repository.GetByIdWithRolAsync(id);
        return MapToDto(actualizadoConRol!);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }

    // BCrypt genera una "sal" distinta en cada hash automáticamente, por eso dos contraseñas
    // iguales nunca producen el mismo hash. AuthServicio.LoginAsync usa BCrypt.Verify para comparar.
    private static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    private static UsuarioDto MapToDto(Usuario usuario)
    {
        return new UsuarioDto
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Email = usuario.Email,
            Activo = usuario.Activo,
            UltimoAcceso = usuario.UltimoAcceso,
            RolId = usuario.RolId,
            RolNombre = usuario.Rol?.Nombre ?? string.Empty,
            CreatedAt = usuario.CreatedAt
        };
    }
}
