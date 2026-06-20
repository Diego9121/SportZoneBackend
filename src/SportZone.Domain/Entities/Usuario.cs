namespace SportZone.Domain.Entities;

// Implementa IActivable: demuestra polimorfismo - cualquier servicio que reciba IActivable
// puede leer/cambiar Activo sin saber que es específicamente un Usuario.
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

    public ICollection<Venta> Ventas { get; set; } = new List<Venta>();
    public ICollection<Ingreso> Ingresos { get; set; } = new List<Ingreso>();
    public ICollection<Devolucion> Devoluciones { get; set; } = new List<Devolucion>();
    public ICollection<Bitacora> Bitacoras { get; set; } = new List<Bitacora>();
}
