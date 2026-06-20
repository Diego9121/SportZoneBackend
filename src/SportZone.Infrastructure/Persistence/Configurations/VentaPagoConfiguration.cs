namespace SportZone.Infrastructure.Persistence.Configurations;

public class VentaPagoConfiguration : BaseEntityConfiguration<VentaPago>
{
    public override void Configure(EntityTypeBuilder<VentaPago> builder)
    {
        base.Configure(builder);
        builder.ToTable("VentaPagos");
        builder.Property(e => e.MetodoPago).HasMaxLength(30).IsRequired();
        builder.Property(e => e.Monto).HasPrecision(10, 2).IsRequired();
        builder.Property(e => e.Referencia).HasMaxLength(100);

        builder.HasOne(e => e.Venta)
               .WithMany(v => v.VentaPagos)
               .HasForeignKey(e => e.VentaId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
