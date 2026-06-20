namespace SportZone.Infrastructure.Persistence.Configurations;

public class ArticuloConfiguration : BaseEntityConfiguration<Articulo>
{
    public override void Configure(EntityTypeBuilder<Articulo> builder)
    {
        base.Configure(builder);
        builder.ToTable("Articulos");
        builder.Property(e => e.Codigo).HasMaxLength(50).IsRequired();
        builder.HasIndex(e => e.Codigo).IsUnique();
        builder.Property(e => e.Nombre).HasMaxLength(150).IsRequired();
        builder.Property(e => e.Imagen).HasMaxLength(500);

        builder.HasOne(e => e.Categoria)
               .WithMany(c => c.Articulos)
               .HasForeignKey(e => e.CategoriaId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Marca)
               .WithMany(m => m.Articulos)
               .HasForeignKey(e => e.MarcaId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
