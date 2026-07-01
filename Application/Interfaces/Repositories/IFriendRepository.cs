using LinkUpProject.Domain.Entities;

namespace LinkUpProject.Application.Interfaces.Repositories;

public interface IFriendRepository
{
    Task<List<ApplicationUser>> SearchActiveUsersAsync(string currentUserId, string? searchTerm);
    Task<List<FriendRequest>> GetIncomingPendingRequestsAsync(string userId);
    Task<List<FriendRequest>> GetOutgoingPendingRequestsAsync(string userId);
    Task<List<Friendship>> GetActiveFriendshipsAsync(string userId);
    Task<FriendRequest?> GetRequestByIdAsync(int requestId);
    Task<bool> HasActiveFriendshipAsync(string userId, string otherUserId);
    Task<bool> HasPendingRequestBetweenAsync(string userId, string otherUserId);
    Task<bool> SendRequestAsync(FriendRequest request);
    Task<bool> AcceptRequestAsync(FriendRequest request, Friendship friendship);
    Task UpdateRequestAsync(FriendRequest request);
    Task UpdateFriendshipAsync(Friendship friendship);
}
