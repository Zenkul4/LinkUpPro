using LinkUpProject.Application.ViewModels.Battleship;
using LinkUpProject.Domain.Common;

namespace LinkUpProject.Application.Interfaces.Services;

public interface IBattleshipService
{
    Task<Result<BattleshipIndexViewModel>> GetIndexAsync(string userId);
    Task<Result<int>> CreateMatchAsync(string playerId, string opponentId);
    Task<Result<BattleshipSetupViewModel>> GetSetupAsync(int matchId, string userId);
    Task<Result> SaveShipsAsync(BattleshipSetupViewModel viewModel, string userId);
}
