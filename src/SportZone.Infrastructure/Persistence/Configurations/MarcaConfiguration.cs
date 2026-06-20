namespace SportZone.Infrastructure.Persistence.Configurations;

public class MarcaConfiguration : BaseEntityConfiguration<Marca>
{
    public override void Configure(EntityTypeBuilder<Marca> builder)
    {
        base.Configure(builder);
        builder.ToTable("Marcas");
        builder.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Descripcion).HasMaxLength(255);
        builder.Property(e => e.Logo).HasMaxLength(500);
    }
}
