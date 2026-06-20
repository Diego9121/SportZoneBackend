namespace SportZone.Infrastructure.Persistence.Configurations;

public class ProveedorConfiguration : BaseEntityConfiguration<Proveedor>
{
    public override void Configure(EntityTypeBuilder<Proveedor> builder)
    {
        base.Configure(builder);
        builder.ToTable("Proveedores");
        builder.Property(e => e.Nombre).HasMaxLength(150).IsRequired();
        builder.Property(e => e.Contacto).HasMaxLength(100);
        builder.Property(e => e.Telefono).HasMaxLength(20);
        builder.Property(e => e.Email).HasMaxLength(100);
        builder.Property(e => e.Direccion).HasMaxLength(255);
    }
}
