using LinkUpProject.Application.Interfaces.Repositories;
using LinkUpProject.Domain.Entities;
using LinkUpProject.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace LinkUpProject.Persistence.Repositories;

public class FriendRepository : IFriendRepository
{
    private const string PendingStatus = "Pending";
    private const string ActiveStatus = "Active";
    private readonly ApplicationDbContext _dbContext;

    public FriendRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ApplicationUser>> SearchActiveUsersAsync(string currentUserId, string? searchTerm)
    {
        var query = _dbContext.Users
            .AsNoTracking()
            .Where(u => u.Id != currentUserId && u.IsActive && u.EmailConfirmed);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(u =>
                u.FirstName.Contains(term) ||
                u.LastName.Contains(term) ||
                u.UserName!.Contains(term) ||
                u.Email!.Contains(term));
        }

        return await query
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .Take(25)
            .ToListAsync();
    }

    public async Task<List<FriendRequest>> GetIncomingPendingRequestsAsync(string userId)
    {
        return await _dbContext.FriendRequests
            .AsNoTracking()
            .Include(fr => fr.Sender)
            .Include(fr => fr.Receiver)
            .Where(fr => fr.ReceiverId == userId && fr.Status == PendingStatus)
            .OrderByDescending(fr => fr.SentAt)
            .ToListAsync();
    }

    public async Task<List<FriendRequest>> GetOutgoingPendingRequestsAsync(string userId)
    {
        return await _dbContext.FriendRequests
            .AsNoTracking()
            .Include(fr => fr.Sender)
            .Include(fr => fr.Receiver)
            .Where(fr => fr.SenderId == userId && fr.Status == PendingStatus && !fr.IsHiddenBySender)
            .OrderByDescending(fr => fr.SentAt)
            .ToListAsync();
    }

    public async Task<List<Friendship>> GetActiveFriendshipsAsync(string userId)
    {
        return await _dbContext.Friendships
            .AsNoTracking()
            .Include(f => f.User1)
            .Include(f => f.User2)
            .Where(f => f.Status == ActiveStatus && (f.User1Id == userId || f.User2Id == userId))
            .OrderBy(f => f.User1.FirstName)
            .ThenBy(f => f.User2.FirstName)
            .ToListAsync();
    }

    public async Task<FriendRequest?> GetRequestByIdAsync(int requestId)
    {
        return await _dbContext.FriendRequests
            .Include(fr => fr.Sender)
            .Include(fr => fr.Receiver)
            .FirstOrDefaultAsync(fr => fr.Id == requestId);
    }

    public async Task<bool> HasActiveFriendshipAsync(string userId, string otherUserId)
    {
        var (user1Id, user2Id) = NormalizePair(userId, otherUserId);

        return await _dbContext.Friendships
            .AnyAsync(f => f.User1Id == user1Id && f.User2Id == user2Id && f.Status == ActiveStatus);
    }

    public async Task<bool> HasPendingRequestBetweenAsync(string userId, string otherUserId)
    {
        return await _dbContext.FriendRequests.AnyAsync(fr =>
            fr.Status == PendingStatus &&
            ((fr.SenderId == userId && fr.ReceiverId == otherUserId) ||
             (fr.SenderId == otherUserId && fr.ReceiverId == userId)));
    }

    public async Task<bool> SendRequestAsync(FriendRequest request)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);

        if (await HasActiveFriendshipAsync(request.SenderId, request.ReceiverId) ||
            await HasPendingRequestBetweenAsync(request.SenderId, request.ReceiverId))
        {
            return false;
        }

        await _dbContext.FriendRequests.AddAsync(request);
        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();

        return true;
    }

    public async Task<bool> AcceptRequestAsync(FriendRequest request, Friendship friendship)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);

        var existingFriendship = await _dbContext.Friendships.FirstOrDefaultAsync(f =>
            f.User1Id == friendship.User1Id && f.User2Id == friendship.User2Id);

        if (existingFriendship?.Status == ActiveStatus)
        {
            return false;
        }

        if (existingFriendship == null)
        {
            await _dbContext.Friendships.AddAsync(friendship);
        }
        else
        {
            existingFriendship.Status = ActiveStatus;
            existingFriendship.CreatedAt = DateTime.UtcNow;
        }

        _dbContext.FriendRequests.Update(request);
        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();

        return true;
    }

    public async Task UpdateRequestAsync(FriendRequest request)
    {
        _dbContext.FriendRequests.Update(request);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateFriendshipAsync(Friendship friendship)
    {
        _dbContext.Friendships.Update(friendship);
        await _dbContext.SaveChangesAsync();
    }

    private static (string User1Id, string User2Id) NormalizePair(string firstUserId, string secondUserId)
    {
        return string.CompareOrdinal(firstUserId, secondUserId) <= 0
            ? (firstUserId, secondUserId)
            : (secondUserId, firstUserId);
    }
}
