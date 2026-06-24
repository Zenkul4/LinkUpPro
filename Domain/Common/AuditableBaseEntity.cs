namespace LinkUpProject.Domain.Common;

public abstract class AuditableBaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
}