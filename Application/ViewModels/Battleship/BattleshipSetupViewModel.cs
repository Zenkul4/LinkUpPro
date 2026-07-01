namespace LinkUpProject.Application.ViewModels.Battleship;

public class BattleshipSetupViewModel
{
    public int MatchId { get; set; }
    public string OpponentName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public IReadOnlyList<int> RequiredShipSizes { get; set; } = [];
    public List<SaveBattleshipShipViewModel> Ships { get; set; } = [];
    public bool CurrentUserHasPlacedShips { get; set; }
}
