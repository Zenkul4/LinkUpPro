namespace LinkUpProject.Domain.Entities;

public class FriendRequest
{
    public int Id { get; set; }
    public string SenderId { get; set; } = string.Empty;
    public ApplicationUser Sender { get; set; } = null!;

    public string ReceiverId { get; set; } = string.Empty;
    public ApplicationUser Receiver { get; set; } = null!;

    public string Status { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public DateTime? RespondedAt { get; set; }
    public bool IsHiddenBySender { get; set; }
}