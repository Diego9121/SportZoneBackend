namespace SportZone.Infrastructure.Persistence.Configurations;

public class RolConfiguration : BaseEntityConfiguration<Rol>
{
    public override void Configure(EntityTypeBuilder<Rol> builder)
    {
        base.Configure(builder);
        builder.ToTable("Roles");
        builder.Property(e => e.Nombre).HasMaxLength(50).IsRequired();
        builder.Property(e => e.Descripcion).HasMaxLength(255);
    }
}
