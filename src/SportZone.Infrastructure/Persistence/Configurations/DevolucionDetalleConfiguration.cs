namespace SportZone.Infrastructure.Persistence.Configurations;

public class DevolucionDetalleConfiguration : BaseEntityConfiguration<DevolucionDetalle>
{
    public override void Configure(EntityTypeBuilder<DevolucionDetalle> builder)
    {
        base.Configure(builder);
        builder.ToTable("DevolucionDetalles");
        builder.Property(e => e.PrecioUnitario).HasPrecision(10, 2).IsRequired();
        builder.Property(e => e.Subtotal).HasPrecision(10, 2).IsRequired();

        builder.HasOne(e => e.Devolucion)
               .WithMany(d => d.DevolucionDetalles)
               .HasForeignKey(e => e.DevolucionId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Variante)
               .WithMany(v => v.DevolucionDetalles)
               .HasForeignKey(e => e.VarianteId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
