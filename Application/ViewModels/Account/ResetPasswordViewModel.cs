using System.ComponentModel.DataAnnotations;

namespace LinkUpProject.Application.ViewModels.Account;

public class ResetPasswordViewModel
{
    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    public string Token { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe ingresar la nueva contraseña.")]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe confirmar la contraseña.")]
    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword), ErrorMessage = "La nueva contraseña y su confirmación no coinciden.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}