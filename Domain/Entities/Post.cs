using LinkUpProject.Domain.Common;
using System.Xml.Linq;
using static System.Collections.Specialized.BitVector32;

namespace LinkUpProject.Domain.Entities;

public class Post : AuditableBaseEntity
{
    public string Content { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string? MediaUrl { get; set; }
    public string Privacy { get; set; } = string.Empty;
    public bool AllowComments { get; set; }
    public bool IsDeleted { get; set; }

    public string AuthorId { get; set; } = string.Empty;
    public ApplicationUser Author { get; set; } = null!;

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();
}