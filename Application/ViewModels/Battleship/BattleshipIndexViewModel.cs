namespace LinkUpProject.Application.ViewModels.Battleship;

public class BattleshipIndexViewModel
{
    public IReadOnlyList<BattleshipOpponentViewModel> Opponents { get; set; } = [];
    public IReadOnlyList<BattleshipMatchSummaryViewModel> Matches { get; set; } = [];
}
