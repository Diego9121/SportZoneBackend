namespace SportZone.Infrastructure.Persistence.Configurations;

public class ClienteConfiguration : BaseEntityConfiguration<Cliente>
{
    public override void Configure(EntityTypeBuilder<Cliente> builder)
    {
        base.Configure(builder);
        builder.ToTable("Clientes");
        builder.Property(e => e.TipoDocumento).HasMaxLength(20);
        builder.Property(e => e.Documento).HasMaxLength(20);
        builder.HasIndex(e => e.Documento).IsUnique();
        builder.Property(e => e.Nombre).HasMaxLength(150).IsRequired();
        builder.Property(e => e.Telefono).HasMaxLength(20);
        builder.Property(e => e.Email).HasMaxLength(100);
        builder.Property(e => e.Direccion).HasMaxLength(255);
    }
}
