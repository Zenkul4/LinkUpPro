using System.ComponentModel.DataAnnotations;

namespace LinkUpProject.Application.ViewModels.Comment;

public class SaveCommentViewModel
{
    public int Id { get; set; }

    public int PostId { get; set; }

    public int? ParentCommentId { get; set; }

    [Required(ErrorMessage = "El contenido del comentario es requerido.")]
    [StringLength(500, ErrorMessage = "El comentario no puede superar los 500 caracteres.")]
    [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "El comentario no puede contener únicamente espacios.")]
    public string Content { get; set; } = string.Empty;
}