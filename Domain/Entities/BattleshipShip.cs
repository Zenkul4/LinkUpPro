namespace LinkUpProject.Domain.Entities;

public class BattleshipShip
{
    public int Id { get; set; }
    public int MatchId { get; set; }
    public BattleshipMatch Match { get; set; } = null!;

    public string PlayerId { get; set; } = string.Empty;
    public int Size { get; set; }
    public int StartX { get; set; }
    public int StartY { get; set; }
    public string Direction { get; set; } = string.Empty;
    public bool IsSunk { get; set; }
}