namespace LinkUpProject.Domain.Entities;

public class BattleshipMatch
{
    public int Id { get; set; }
    public string Player1Id { get; set; } = string.Empty;
    public ApplicationUser Player1 { get; set; } = null!;

    public string Player2Id { get; set; } = string.Empty;
    public ApplicationUser Player2 { get; set; } = null!;

    public string CurrentTurnId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? WinnerId { get; set; }

    public DateTime StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public DateTime LastActivityAt { get; set; }

    public ICollection<BattleshipShip> Ships { get; set; } = new List<BattleshipShip>();
    public ICollection<BattleshipAttack> Attacks { get; set; } = new List<BattleshipAttack>();
}