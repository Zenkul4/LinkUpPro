using LinkUpProject.Application.Validations;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace LinkUpProject.Application.ViewModels.Post;

public class SavePostViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Debe ingresar el contenido de la publicación.")]
    [StringLength(1000, ErrorMessage = "El contenido no puede superar los 1000 caracteres.")]
    [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "Debe ingresar el contenido de la publicación.")]
    public string Content { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe seleccionar el tipo de contenido.")]
    public string ContentType { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe seleccionar la privacidad.")]
    public string Privacy { get; set; } = "Solo amigos";
    public bool AllowComments { get; set; }

    [MaxFileSize(5 * 1024 * 1024)]
    [AllowedExtensions(new[] { ".jpg", ".jpeg", ".png", ".webp" })]
    public IFormFile? MediaFile { get; set; }

    [Url(ErrorMessage = "Debe ingresar un enlace válido de YouTube.")]
    public string? YouTubeLink { get; set; }

    public string? ExistingMediaUrl { get; set; }
}