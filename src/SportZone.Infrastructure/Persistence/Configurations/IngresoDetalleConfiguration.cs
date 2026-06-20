namespace SportZone.Infrastructure.Persistence.Configurations;

public class IngresoDetalleConfiguration : BaseEntityConfiguration<IngresoDetalle>
{
    public override void Configure(EntityTypeBuilder<IngresoDetalle> builder)
    {
        base.Configure(builder);
        builder.ToTable("IngresoDetalles");
        builder.Property(e => e.PrecioCosto).HasPrecision(10, 2).IsRequired();
        builder.Property(e => e.Subtotal).HasPrecision(10, 2).IsRequired();

        builder.HasOne(e => e.Ingreso)
               .WithMany(i => i.IngresoDetalles)
               .HasForeignKey(e => e.IngresoId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Variante)
               .WithMany(v => v.IngresoDetalles)
               .HasForeignKey(e => e.VarianteId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
