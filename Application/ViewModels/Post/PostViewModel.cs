using LinkUpProject.Application.ViewModels.Comment;

namespace LinkUpProject.Application.ViewModels.Post;

public class PostViewModel
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string? MediaUrl { get; set; }
    public string Privacy { get; set; } = string.Empty;
    public bool AllowComments { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public bool IsEdited => LastModifiedAt.HasValue;

    public string AuthorId { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorUsername { get; set; } = string.Empty;
    public string? AuthorProfilePictureUrl { get; set; }

    public int LikesCount { get; set; }
    public int DislikesCount { get; set; }
    public string? CurrentUserReaction { get; set; }

    public ICollection<CommentViewModel> Comments { get; set; } = new List<CommentViewModel>();
}