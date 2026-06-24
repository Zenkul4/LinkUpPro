using LinkUpProject.Domain.Common;

namespace LinkUpProject.Domain.Entities;

public class Comment : AuditableBaseEntity
{
    public string Content { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }

    public int PostId { get; set; }
    public Post Post { get; set; } = null!;

    public string AuthorId { get; set; } = string.Empty;
    public ApplicationUser Author { get; set; } = null!;

    public int? ParentCommentId { get; set; }
    public Comment? ParentComment { get; set; }
    public ICollection<Comment> Replies { get; set; } = new List<Comment>();
}