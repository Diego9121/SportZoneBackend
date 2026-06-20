namespace SportZone.Domain.Entities;

public class ProveedorMarca : BaseEntity
{
    public int ProveedorId { get; set; }
    public int MarcaId { get; set; }

    public Proveedor Proveedor { get; set; } = null!;
    public Marca Marca { get; set; } = null!;
}
