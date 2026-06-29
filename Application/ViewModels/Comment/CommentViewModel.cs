namespace LinkUpProject.Application.ViewModels.Comment;

public class CommentViewModel
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public int? ParentCommentId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public bool IsEdited => LastModifiedAt.HasValue;

    public string AuthorId { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorUsername { get; set; } = string.Empty;
    public string? AuthorProfilePictureUrl { get; set; }

    public ICollection<CommentViewModel> Replies { get; set; } = new List<CommentViewModel>();
}