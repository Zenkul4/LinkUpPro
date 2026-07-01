namespace LinkUpProject.Application.ViewModels.Battleship;

public class BattleshipMatchSummaryViewModel
{
    public int Id { get; set; }
    public string OpponentName { get; set; } = string.Empty;
    public string OpponentUserName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool CurrentUserHasPlacedShips { get; set; }
    public int CurrentUserShipsCount { get; set; }
    public int OpponentShipsCount { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime LastActivityAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public int ElapsedHours { get; set; }
    public bool IsFinished { get; set; }
}
