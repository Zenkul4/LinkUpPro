using LinkUpProject.Application.ViewModels.Account;
using LinkUpProject.Domain.Common;

namespace LinkUpProject.Application.Interfaces.Services;

public interface IAccountService
{
    Task<Result> LoginAsync(LoginViewModel vm);
    Task<Result> RegisterAsync(RegisterViewModel vm);
    Task LogoutAsync();

    Task<Result> ForgotPasswordAsync(ForgotPasswordViewModel vm);
    Task<Result> ResetPasswordAsync(ResetPasswordViewModel vm);

    Task<Result<ProfileViewModel>> GetProfileAsync(string userName);
    Task<Result> ConfirmEmailAsync(string userId, string token);
    Task<Result> UpdateProfileAsync(ProfileViewModel vm, string userId);
}