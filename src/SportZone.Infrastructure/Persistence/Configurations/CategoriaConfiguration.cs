namespace SportZone.Infrastructure.Persistence.Configurations;

public class CategoriaConfiguration : BaseEntityConfiguration<Categoria>
{
    public override void Configure(EntityTypeBuilder<Categoria> builder)
    {
        base.Configure(builder);
        builder.ToTable("Categorias");
        builder.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Descripcion).HasMaxLength(255);
    }
}
