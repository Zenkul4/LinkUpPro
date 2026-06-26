using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.Account;

public class ForgotPasswordViewModel
{
    [Required(ErrorMessage = "Debe ingresar el nombre de usuario.")]
    public string UserName { get; set; } = string.Empty;
}