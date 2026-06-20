namespace SportZone.Infrastructure.Persistence.Configurations;

public class MovimientoStockConfiguration : BaseEntityConfiguration<MovimientoStock>
{
    public override void Configure(EntityTypeBuilder<MovimientoStock> builder)
    {
        base.Configure(builder);
        builder.ToTable("MovimientosStock");
        builder.Property(e => e.TipoMovimiento).HasMaxLength(100).IsRequired();
        builder.Property(e => e.NumeroDoc).HasMaxLength(50);

        builder.HasOne(e => e.ArticuloVariante)
               .WithMany(v => v.MovimientosStock)
               .HasForeignKey(e => e.ArticuloVarianteId)
               .OnDelete(DeleteBehavior.Restrict);

        // Ingreso/Venta son opcionales (un movimiento podría venir de un ajuste manual sin documento origen)
        builder.HasOne(e => e.Ingreso)
               .WithMany(i => i.MovimientosStock)
               .HasForeignKey(e => e.IngresoId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Venta)
               .WithMany(v => v.MovimientosStock)
               .HasForeignKey(e => e.VentaId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
