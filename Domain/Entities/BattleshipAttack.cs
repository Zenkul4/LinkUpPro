namespace LinkUpProject.Domain.Entities;

public class BattleshipAttack
{
    public int Id { get; set; }
    public int MatchId { get; set; }
    public BattleshipMatch Match { get; set; } = null!;

    public string AttackerId { get; set; } = string.Empty;
    public int X { get; set; }
    public int Y { get; set; }
    public bool IsHit { get; set; }
    public DateTime AttackedAt { get; set; }
}