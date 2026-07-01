using LinkUpProject.Domain.Entities;

namespace LinkUpProject.Application.Interfaces.Repositories;

public interface IBattleshipRepository
{
    Task<List<Friendship>> GetActiveFriendshipsAsync(string userId);
    Task<List<BattleshipMatch>> GetMatchesForUserAsync(string userId);
    Task<BattleshipMatch?> GetMatchByIdAsync(int matchId);
    Task<bool> HasOpenMatchAsync(string playerId, string opponentId);
    Task<BattleshipMatch?> AddMatchAsync(BattleshipMatch match);
    Task ReplaceShipsAsync(int matchId, string playerId, IReadOnlyList<BattleshipShip> ships);
    Task UpdateMatchAsync(BattleshipMatch match);
}
