namespace SportZone.Infrastructure.Persistence.Configurations;

public class VentaDetalleConfiguration : BaseEntityConfiguration<VentaDetalle>
{
    public override void Configure(EntityTypeBuilder<VentaDetalle> builder)
    {
        base.Configure(builder);
        builder.ToTable("VentaDetalles");
        builder.Property(e => e.PrecioUnitario).HasPrecision(10, 2).IsRequired();
        builder.Property(e => e.Descuento).HasPrecision(10, 2).IsRequired();
        builder.Property(e => e.Subtotal).HasPrecision(10, 2).IsRequired();

        builder.HasOne(e => e.Venta)
               .WithMany(v => v.VentaDetalles)
               .HasForeignKey(e => e.VentaId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Variante)
               .WithMany(v => v.VentaDetalles)
               .HasForeignKey(e => e.VarianteId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
