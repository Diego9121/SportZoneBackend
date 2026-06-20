namespace SportZone.Domain.Entities;

// Implementa IActivable: demuestra polimorfismo - cualquier servicio que reciba IActivable
// puede leer/cambiar Activo sin saber que es específicamente un Usuario.
// Ya no tiene colecciones de Venta/Ingreso/Devolucion/Bitacora: esas tablas dejaron de
// referenciar Usuario directamente (queda cubierto por CreateById de BaseEntity).
public class Usuario : BaseEntity, IActivable
{
    public int RolId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? TokenRefresh { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime? UltimoAcceso { get; set; }

    public Rol Rol { get; set; } = null!;
}
