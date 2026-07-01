namespace LinkUpProject.Application.ViewModels.Friend;

public class FriendRequestViewModel
{
    public int Id { get; set; }
    public string SenderId { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string SenderUserName { get; set; } = string.Empty;
    public string ReceiverId { get; set; } = string.Empty;
    public string ReceiverName { get; set; } = string.Empty;
    public string ReceiverUserName { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
}
