using System.ComponentModel.DataAnnotations;

namespace LinkUpProject.Application.ViewModels.Reaction;

public class ReactionViewModel
{
    [Required]
    public int PostId { get; set; }

    [Required]
    public string Type { get; set; } = string.Empty;
}