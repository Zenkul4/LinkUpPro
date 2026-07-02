namespace LinkUpProject.Application.ViewModels.Battleship;

public class BattleshipGameplayViewModel
{
    public int MatchId { get; set; }
    public string OpponentName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsMyTurn { get; set; }
    public string? WinnerId { get; set; }
    public string CurrentUserId { get; set; } = string.Empty;

    // Listas para dibujar los tableros
    public List<BattleshipShipViewModel> MyShips { get; set; } = new();
    public List<BattleshipAttackViewModel> MyAttacks { get; set; } = new();
    public List<BattleshipAttackViewModel> OpponentAttacks { get; set; } = new();
}