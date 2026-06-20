namespace SportZone.Domain.Common;

public abstract class BaseEntity
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public int CreateById { get; set; }
    public int? UpdateById { get; set; }
    public int? DeleteById { get; set; }

    public bool IsDeleted => DeletedAt.HasValue;
}