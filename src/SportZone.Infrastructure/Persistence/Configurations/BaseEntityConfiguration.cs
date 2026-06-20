namespace SportZone.Infrastructure.Persistence.Configurations;

public abstract class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T>
    where T : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.CreatedAt).IsRequired();
        builder.Property(e => e.UpdatedAt).IsRequired(false);
        builder.Property(e => e.DeletedAt).IsRequired(false);

        builder.Property(e => e.CreateById).IsRequired();
        builder.Property(e => e.UpdateById).IsRequired(false);
        builder.Property(e => e.DeleteById).IsRequired(false);

        builder.Ignore(e => e.IsDeleted);

        builder.HasQueryFilter(e => e.DeletedAt == null);
    }
}
