namespace SportZone.Infrastructure.Persistence.Configurations;

public class BitacoraConfiguration : BaseEntityConfiguration<Bitacora>
{
    public override void Configure(EntityTypeBuilder<Bitacora> builder)
    {
        base.Configure(builder);
        builder.ToTable("Bitacoras");
        builder.Property(e => e.Accion).HasMaxLength(100).IsRequired();
        builder.Property(e => e.TablaAfectada).HasMaxLength(50);
        builder.Property(e => e.IpAddress).HasMaxLength(45);

        builder.HasOne(e => e.Usuario)
               .WithMany(u => u.Bitacoras)
               .HasForeignKey(e => e.UsuarioId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
