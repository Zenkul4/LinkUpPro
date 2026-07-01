using System.ComponentModel.DataAnnotations;

namespace LinkUpProject.Application.ViewModels.Battleship;

public class SaveBattleshipShipViewModel
{
    [Required]
    public int Size { get; set; }

    [Range(1, 12)]
    public int StartX { get; set; }

    [Range(1, 12)]
    public int StartY { get; set; }

    [Required]
    public string Direction { get; set; } = "Horizontal";
}
