namespace LinkUpProject.Application.ViewModels.Friend;

public class FriendDashboardViewModel
{
    public string? SearchTerm { get; set; }
    public IReadOnlyList<FriendUserViewModel> SearchResults { get; set; } = [];
    public IReadOnlyList<FriendRequestViewModel> IncomingRequests { get; set; } = [];
    public IReadOnlyList<FriendRequestViewModel> OutgoingRequests { get; set; } = [];
    public IReadOnlyList<FriendUserViewModel> Friends { get; set; } = [];
}
