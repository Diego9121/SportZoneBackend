namespace SportZone.Infrastructure.Persistence.Configurations;

public class ProveedorMarcaConfiguration : BaseEntityConfiguration<ProveedorMarca>
{
    public override void Configure(EntityTypeBuilder<ProveedorMarca> builder)
    {
        base.Configure(builder);
        builder.ToTable("ProveedorMarcas");
        builder.HasIndex(e => new { e.ProveedorId, e.MarcaId }).IsUnique();

        builder.HasOne(e => e.Proveedor)
               .WithMany(p => p.ProveedorMarcas)
               .HasForeignKey(e => e.ProveedorId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Marca)
               .WithMany(m => m.ProveedorMarcas)
               .HasForeignKey(e => e.MarcaId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
