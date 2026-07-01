using LinkUpProject.Application.Interfaces.Repositories;
using LinkUpProject.Domain.Entities;
using LinkUpProject.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace LinkUpProject.Persistence.Repositories;

public class BattleshipRepository : IBattleshipRepository
{
    private const string ActiveFriendshipStatus = "Active";
    private readonly ApplicationDbContext _dbContext;

    public BattleshipRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Friendship>> GetActiveFriendshipsAsync(string userId)
    {
        return await _dbContext.Friendships
            .AsNoTracking()
            .Include(f => f.User1)
            .Include(f => f.User2)
            .Where(f => f.Status == ActiveFriendshipStatus && (f.User1Id == userId || f.User2Id == userId))
            .ToListAsync();
    }

    public async Task<List<BattleshipMatch>> GetMatchesForUserAsync(string userId)
    {
        return await _dbContext.BattleshipMatches
            .AsNoTracking()
            .Include(m => m.Player1)
            .Include(m => m.Player2)
            .Include(m => m.Ships)
            .Where(m => m.Player1Id == userId || m.Player2Id == userId)
            .OrderByDescending(m => m.LastActivityAt)
            .ToListAsync();
    }

    public async Task<BattleshipMatch?> GetMatchByIdAsync(int matchId)
    {
        return await _dbContext.BattleshipMatches
            .Include(m => m.Player1)
            .Include(m => m.Player2)
            .Include(m => m.Ships)
            .FirstOrDefaultAsync(m => m.Id == matchId);
    }

    public async Task<bool> HasOpenMatchAsync(string playerId, string opponentId)
    {
        return await _dbContext.BattleshipMatches.AnyAsync(m =>
            m.FinishedAt == null &&
            ((m.Player1Id == playerId && m.Player2Id == opponentId) ||
             (m.Player1Id == opponentId && m.Player2Id == playerId)));
    }

    public async Task<BattleshipMatch?> AddMatchAsync(BattleshipMatch match)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);

        if (await HasOpenMatchAsync(match.Player1Id, match.Player2Id))
            return null;

        await _dbContext.BattleshipMatches.AddAsync(match);
        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();

        return match;
    }

    public async Task ReplaceShipsAsync(int matchId, string playerId, IReadOnlyList<BattleshipShip> ships)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);

        var existingShips = await _dbContext.BattleshipShips
            .Where(s => s.MatchId == matchId && s.PlayerId == playerId)
            .ToListAsync();

        _dbContext.BattleshipShips.RemoveRange(existingShips);
        await _dbContext.BattleshipShips.AddRangeAsync(ships);
        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    public async Task UpdateMatchAsync(BattleshipMatch match)
    {
        _dbContext.BattleshipMatches.Update(match);
        await _dbContext.SaveChangesAsync();
    }
}
