namespace LinkUpProject.Domain.Entities;

public class Notification
{
    public int Id { get; set; }
    public string ReceiverId { get; set; } = string.Empty;
    public ApplicationUser Receiver { get; set; } = null!;

    public string SenderId { get; set; } = string.Empty;
    public ApplicationUser Sender { get; set; } = null!;

    public string ActivityType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
    public int? RelatedContentId { get; set; }
}