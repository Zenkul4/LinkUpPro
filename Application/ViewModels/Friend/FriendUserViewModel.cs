namespace LinkUpProject.Application.ViewModels.Friend;

public class FriendUserViewModel
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public int MutualFriendsCount { get; set; }
    public string RelationshipStatus { get; set; } = string.Empty;
}
