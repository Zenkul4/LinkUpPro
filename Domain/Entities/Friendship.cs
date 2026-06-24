namespace LinkUpProject.Domain.Entities;

public class Friendship
{
    public int Id { get; set; }
    public string User1Id { get; set; } = string.Empty;
    public ApplicationUser User1 { get; set; } = null!;

    public string User2Id { get; set; } = string.Empty;
    public ApplicationUser User2 { get; set; } = null!;

    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}