using LinkUpProject.Application.ViewModels.Friend;
using LinkUpProject.Domain.Common;

namespace LinkUpProject.Application.Interfaces.Services;

public interface IFriendService
{
    Task<Result<FriendDashboardViewModel>> GetDashboardAsync(string userId, string? searchTerm);
    Task<Result> SendRequestAsync(string senderId, string receiverId);
    Task<Result> AcceptRequestAsync(string userId, int requestId);
    Task<Result> RejectRequestAsync(string userId, int requestId);
    Task<Result> CancelRequestAsync(string userId, int requestId);
    Task<Result> RemoveFriendAsync(string userId, string friendId);
}
