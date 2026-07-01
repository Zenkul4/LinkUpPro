using LinkUpProject.Application.Interfaces.Repositories;
using LinkUpProject.Application.Interfaces.Services;
using LinkUpProject.Application.ViewModels.Friend;
using LinkUpProject.Domain.Common;
using LinkUpProject.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace LinkUpProject.Application.Services;

public class FriendService : IFriendService
{
    private const string PendingStatus = "Pending";
    private const string AcceptedStatus = "Accepted";
    private const string RejectedStatus = "Rejected";
    private const string CancelledStatus = "Cancelled";
    private const string ActiveStatus = "Active";
    private const string DeletedStatus = "Deleted";

    private readonly IFriendRepository _friendRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public FriendService(IFriendRepository friendRepository, UserManager<ApplicationUser> userManager)
    {
        _friendRepository = friendRepository;
        _userManager = userManager;
    }

    public async Task<Result<FriendDashboardViewModel>> GetDashboardAsync(string userId, string? searchTerm)
    {
        var incomingRequests = await _friendRepository.GetIncomingPendingRequestsAsync(userId);
        var outgoingRequests = await _friendRepository.GetOutgoingPendingRequestsAsync(userId);
        var friendships = await _friendRepository.GetActiveFriendshipsAsync(userId);
        var friends = friendships
            .Select(f => GetOtherUser(f, userId))
            .Where(u => u.IsActive && u.EmailConfirmed)
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .ToList();

        var searchResults = await _friendRepository.SearchActiveUsersAsync(userId, searchTerm);
        var currentFriendIds = friends.Select(f => f.Id).ToHashSet();
        var incomingSenderIds = incomingRequests.Select(r => r.SenderId).ToHashSet();
        var outgoingReceiverIds = outgoingRequests.Select(r => r.ReceiverId).ToHashSet();

        var model = new FriendDashboardViewModel
        {
            SearchTerm = searchTerm,
            IncomingRequests = incomingRequests
                .Where(r => r.Sender.IsActive && r.Sender.EmailConfirmed)
                .Select(ToRequestViewModel)
                .ToList(),
            OutgoingRequests = outgoingRequests
                .Where(r => r.Receiver.IsActive && r.Receiver.EmailConfirmed)
                .Select(ToRequestViewModel)
                .ToList(),
            Friends = await BuildFriendViewModelsAsync(userId, friends, "Amigos"),
            SearchResults = await BuildSearchResultViewModelsAsync(
                userId,
                searchResults,
                currentFriendIds,
                incomingSenderIds,
                outgoingReceiverIds)
        };

        return Result<FriendDashboardViewModel>.Success(model);
    }

    public async Task<Result> SendRequestAsync(string senderId, string receiverId)
    {
        if (senderId == receiverId)
            return Result.Failure("No puedes enviarte una solicitud a ti mismo.");

        var receiver = await _userManager.FindByIdAsync(receiverId);
        if (receiver == null || !receiver.IsActive || !receiver.EmailConfirmed)
            return Result.Failure("El usuario seleccionado no esta disponible.");

        var request = new FriendRequest
        {
            SenderId = senderId,
            ReceiverId = receiverId,
            Status = PendingStatus,
            SentAt = DateTime.UtcNow
        };

        var created = await _friendRepository.SendRequestAsync(request);
        if (!created)
            return Result.Failure("Ya existe una solicitud pendiente o una amistad activa con este usuario.");

        return Result.Success();
    }

    public async Task<Result> AcceptRequestAsync(string userId, int requestId)
    {
        var request = await _friendRepository.GetRequestByIdAsync(requestId);
        if (request == null || request.ReceiverId != userId || request.Status != PendingStatus)
            return Result.Failure("La solicitud no existe o no tienes permisos para aceptarla.");

        if (!request.Sender.IsActive || !request.Sender.EmailConfirmed)
            return Result.Failure("La cuenta que envio la solicitud ya no esta activa.");

        var (user1Id, user2Id) = NormalizePair(request.SenderId, request.ReceiverId);
        request.Status = AcceptedStatus;
        request.RespondedAt = DateTime.UtcNow;

        var friendship = new Friendship
        {
            User1Id = user1Id,
            User2Id = user2Id,
            Status = ActiveStatus,
            CreatedAt = DateTime.UtcNow
        };

        var accepted = await _friendRepository.AcceptRequestAsync(request, friendship);
        if (!accepted)
            return Result.Failure("Esta amistad ya existe.");

        return Result.Success();
    }

    public async Task<Result> RejectRequestAsync(string userId, int requestId)
    {
        var request = await _friendRepository.GetRequestByIdAsync(requestId);
        if (request == null || request.ReceiverId != userId || request.Status != PendingStatus)
            return Result.Failure("La solicitud no existe o no tienes permisos para rechazarla.");

        request.Status = RejectedStatus;
        request.RespondedAt = DateTime.UtcNow;

        await _friendRepository.UpdateRequestAsync(request);
        return Result.Success();
    }

    public async Task<Result> CancelRequestAsync(string userId, int requestId)
    {
        var request = await _friendRepository.GetRequestByIdAsync(requestId);
        if (request == null || request.SenderId != userId || request.Status != PendingStatus)
            return Result.Failure("La solicitud no existe o no tienes permisos para cancelarla.");

        request.Status = CancelledStatus;
        request.RespondedAt = DateTime.UtcNow;
        request.IsHiddenBySender = true;

        await _friendRepository.UpdateRequestAsync(request);
        return Result.Success();
    }

    public async Task<Result> RemoveFriendAsync(string userId, string friendId)
    {
        var friendships = await _friendRepository.GetActiveFriendshipsAsync(userId);
        var friendship = friendships.FirstOrDefault(f =>
            (f.User1Id == userId && f.User2Id == friendId) ||
            (f.User1Id == friendId && f.User2Id == userId));

        if (friendship == null)
            return Result.Failure("La amistad no existe o ya fue eliminada.");

        friendship.Status = DeletedStatus;
        await _friendRepository.UpdateFriendshipAsync(friendship);

        return Result.Success();
    }

    private async Task<List<FriendUserViewModel>> BuildFriendViewModelsAsync(
        string currentUserId,
        IReadOnlyList<ApplicationUser> users,
        string relationshipStatus)
    {
        var result = new List<FriendUserViewModel>();

        foreach (var user in users)
        {
            result.Add(new FriendUserViewModel
            {
                Id = user.Id,
                FullName = GetFullName(user),
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                ProfilePictureUrl = user.ProfilePictureUrl,
                RelationshipStatus = relationshipStatus,
                MutualFriendsCount = await CountMutualFriendsAsync(currentUserId, user.Id)
            });
        }

        return result;
    }

    private async Task<List<FriendUserViewModel>> BuildSearchResultViewModelsAsync(
        string currentUserId,
        IReadOnlyList<ApplicationUser> users,
        HashSet<string> currentFriendIds,
        HashSet<string> incomingSenderIds,
        HashSet<string> outgoingReceiverIds)
    {
        var result = new List<FriendUserViewModel>();

        foreach (var user in users)
        {
            var status = string.Empty;
            if (currentFriendIds.Contains(user.Id)) status = "Amigos";
            else if (outgoingReceiverIds.Contains(user.Id)) status = "Solicitud enviada";
            else if (incomingSenderIds.Contains(user.Id)) status = "Solicitud recibida";

            result.Add(new FriendUserViewModel
            {
                Id = user.Id,
                FullName = GetFullName(user),
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                ProfilePictureUrl = user.ProfilePictureUrl,
                RelationshipStatus = status,
                MutualFriendsCount = await CountMutualFriendsAsync(currentUserId, user.Id)
            });
        }

        return result;
    }

    private async Task<int> CountMutualFriendsAsync(string currentUserId, string otherUserId)
    {
        var currentFriendIds = (await _friendRepository.GetActiveFriendshipsAsync(currentUserId))
            .Select(f => GetOtherUser(f, currentUserId))
            .Where(u => u.IsActive && u.EmailConfirmed)
            .Select(u => u.Id)
            .ToHashSet();

        var otherFriendIds = (await _friendRepository.GetActiveFriendshipsAsync(otherUserId))
            .Select(f => GetOtherUser(f, otherUserId))
            .Where(u => u.IsActive && u.EmailConfirmed)
            .Select(u => u.Id)
            .ToHashSet();

        currentFriendIds.IntersectWith(otherFriendIds);
        return currentFriendIds.Count;
    }

    private static FriendRequestViewModel ToRequestViewModel(FriendRequest request)
    {
        return new FriendRequestViewModel
        {
            Id = request.Id,
            SenderId = request.SenderId,
            SenderName = GetFullName(request.Sender),
            SenderUserName = request.Sender.UserName ?? string.Empty,
            ReceiverId = request.ReceiverId,
            ReceiverName = GetFullName(request.Receiver),
            ReceiverUserName = request.Receiver.UserName ?? string.Empty,
            SentAt = request.SentAt
        };
    }

    private static ApplicationUser GetOtherUser(Friendship friendship, string userId)
    {
        return friendship.User1Id == userId ? friendship.User2 : friendship.User1;
    }

    private static string GetFullName(ApplicationUser user)
    {
        return $"{user.FirstName} {user.LastName}".Trim();
    }

    private static (string User1Id, string User2Id) NormalizePair(string firstUserId, string secondUserId)
    {
        return string.CompareOrdinal(firstUserId, secondUserId) <= 0
            ? (firstUserId, secondUserId)
            : (secondUserId, firstUserId);
    }
}
