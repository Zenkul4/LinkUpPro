namespace LinkUpProject.Application.ViewModels.Battleship;

public class BattleshipShipViewModel
{
    public int Size { get; set; }
    public int StartX { get; set; }
    public int StartY { get; set; }
    public string Direction { get; set; } = string.Empty;
    public bool IsSunk { get; set; }
}