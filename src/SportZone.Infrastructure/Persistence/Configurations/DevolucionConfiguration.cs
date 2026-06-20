namespace SportZone.Infrastructure.Persistence.Configurations;

public class DevolucionConfiguration : BaseEntityConfiguration<Devolucion>
{
    public override void Configure(EntityTypeBuilder<Devolucion> builder)
    {
        base.Configure(builder);
        builder.ToTable("Devoluciones");
        builder.Property(e => e.Motivo).HasMaxLength(255).IsRequired();
        builder.Property(e => e.Total).HasPrecision(10, 2).IsRequired();
        builder.Property(e => e.Estado).HasMaxLength(20).IsRequired();

        builder.HasOne(e => e.Venta)
               .WithMany(v => v.Devoluciones)
               .HasForeignKey(e => e.VentaId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Usuario)
               .WithMany(u => u.Devoluciones)
               .HasForeignKey(e => e.UsuarioId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
