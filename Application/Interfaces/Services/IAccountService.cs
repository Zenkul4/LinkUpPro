using Application.ViewModels.Account;

namespace Application.Interfaces.Services;

public interface IAccountService
{
    Task<string?> LoginAsync(LoginViewModel vm);
    Task<string?> RegisterAsync(RegisterViewModel vm);
    Task LogoutAsync();

    Task<string?> ForgotPasswordAsync(ForgotPasswordViewModel vm);
    Task<string?> ResetPasswordAsync(ResetPasswordViewModel vm);

    Task<ProfileViewModel?> GetProfileAsync(string userName);
    Task<string?> ConfirmEmailAsync(string userId, string token);
}