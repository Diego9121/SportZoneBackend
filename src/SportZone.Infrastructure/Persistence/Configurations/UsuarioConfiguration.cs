namespace SportZone.Infrastructure.Persistence.Configurations;

public class UsuarioConfiguration : BaseEntityConfiguration<Usuario>
{
    public override void Configure(EntityTypeBuilder<Usuario> builder)
    {
        base.Configure(builder);
        builder.ToTable("Usuarios");
        builder.Property(e => e.Nombre).HasMaxLength(150).IsRequired();
        builder.Property(e => e.Email).HasMaxLength(100).IsRequired();
        builder.HasIndex(e => e.Email).IsUnique();
        builder.Property(e => e.PasswordHash).HasMaxLength(255).IsRequired();
        builder.Property(e => e.TokenRefresh).HasMaxLength(500);

        builder.HasOne(e => e.Rol)
               .WithMany(r => r.Usuarios)
               .HasForeignKey(e => e.RolId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
