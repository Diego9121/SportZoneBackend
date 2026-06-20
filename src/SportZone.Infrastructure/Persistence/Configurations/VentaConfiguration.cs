namespace SportZone.Infrastructure.Persistence.Configurations;

public class VentaConfiguration : BaseEntityConfiguration<Venta>
{
    public override void Configure(EntityTypeBuilder<Venta> builder)
    {
        base.Configure(builder);
        builder.ToTable("Ventas");
        builder.Property(e => e.NumeroDoc).HasMaxLength(50).IsRequired();
        builder.HasIndex(e => e.NumeroDoc).IsUnique();
        builder.Property(e => e.TipoComprobante).HasMaxLength(20).IsRequired();
        builder.Property(e => e.Subtotal).HasPrecision(10, 2).IsRequired();
        builder.Property(e => e.Descuento).HasPrecision(10, 2).IsRequired();
        builder.Property(e => e.Total).HasPrecision(10, 2).IsRequired();
        builder.Property(e => e.Estado).HasMaxLength(20).IsRequired();

        builder.HasOne(e => e.Cliente)
               .WithMany(c => c.Ventas)
               .HasForeignKey(e => e.ClienteId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.Usuario)
               .WithMany(u => u.Ventas)
               .HasForeignKey(e => e.UsuarioId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
