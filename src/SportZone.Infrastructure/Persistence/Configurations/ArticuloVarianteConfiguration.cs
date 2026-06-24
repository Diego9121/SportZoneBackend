namespace SportZone.Infrastructure.Persistence.Configurations;

public class ArticuloVarianteConfiguration : BaseEntityConfiguration<ArticuloVariante>
{
    public override void Configure(EntityTypeBuilder<ArticuloVariante> builder)
    {
        base.Configure(builder);
        builder.ToTable("ArticuloVariantes");
        builder.Property(e => e.TallaUs).HasMaxLength(10);
        builder.Property(e => e.TallaEu).HasMaxLength(10);
        builder.Property(e => e.TallaUk).HasMaxLength(10);
        builder.Property(e => e.TallaCm).HasMaxLength(10);
        builder.Property(e => e.Color).HasMaxLength(50);
        builder.Property(e => e.CodigoBarras).HasMaxLength(100);
        builder.Property(e => e.ImagenUrl).HasMaxLength(500);
        builder.HasIndex(e => e.CodigoBarras).IsUnique();
        builder.Property(e => e.PrecioVenta).HasPrecision(10, 2).IsRequired();
        builder.Property(e => e.PrecioCosto).HasPrecision(10, 2).IsRequired();

        builder.HasIndex(e => new { e.ArticuloId, e.TallaUs, e.Color }).IsUnique();

        builder.HasOne(e => e.Articulo)
               .WithMany(a => a.Variantes)
               .HasForeignKey(e => e.ArticuloId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
