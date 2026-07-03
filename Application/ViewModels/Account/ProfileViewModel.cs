using System.ComponentModel.DataAnnotations;

namespace LinkUpProject.Application.ViewModels.Account;

public class ProfileViewModel
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede tener mas de 100 caracteres.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(100, ErrorMessage = "El apellido no puede tener mas de 100 caracteres.")]
    public string LastName { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "El telefono no tiene un formato valido.")]
    [StringLength(30, ErrorMessage = "El telefono no puede tener mas de 30 caracteres.")]
    public string PhoneNumber { get; set; } = string.Empty;

    public string? ProfilePictureUrl { get; set; }

    public bool IsEditing { get; set; }
}
