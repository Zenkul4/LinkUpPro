namespace LinkUpProject.Domain.Entities;

public class Reaction
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;

    public int PostId { get; set; }
    public Post Post { get; set; } = null!;

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;
}