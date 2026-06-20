namespace SportZone.Infrastructure.Persistence.Configurations;

public class IngresoConfiguration : BaseEntityConfiguration<Ingreso>
{
    public override void Configure(EntityTypeBuilder<Ingreso> builder)
    {
        base.Configure(builder);
        builder.ToTable("Ingresos");
        builder.Property(e => e.NumeroDoc).HasMaxLength(50);
        builder.Property(e => e.Total).HasPrecision(10, 2).IsRequired();

        builder.HasOne(e => e.Proveedor)
               .WithMany(p => p.Ingresos)
               .HasForeignKey(e => e.ProveedorId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
