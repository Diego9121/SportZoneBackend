namespace SportZone.Domain.Entities;

public class Bitacora : BaseEntity
{
    public int UsuarioId { get; set; }
    public string Accion { get; set; } = string.Empty;
    public string? TablaAfectada { get; set; }
    public int? RegistroId { get; set; }
    public string? Descripcion { get; set; }
    public string? IpAddress { get; set; }

    public Usuario Usuario { get; set; } = null!;
}
