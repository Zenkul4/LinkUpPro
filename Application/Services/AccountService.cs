using Application.Interfaces.Services;
using Application.ViewModels.Account;
using LinkUpProject.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Application.Services;

public class AccountService : IAccountService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<string?> LoginAsync(LoginViewModel vm)
    {
        var user = await _userManager.FindByNameAsync(vm.UserName);

        if (user == null)
            return "El nombre de usuario o la contraseña son incorrectos.";

        if (!user.IsActive || !await _userManager.IsEmailConfirmedAsync(user))
            return "Su cuenta se encuentra inactiva. Debe activarla mediante el enlace enviado a su correo electrónico.";

        var result = await _signInManager.PasswordSignInAsync(
            vm.UserName,
            vm.Password,
            vm.RememberMe,
            lockoutOnFailure: true);

        if (result.IsLockedOut)
            return "La cuenta se encuentra bloqueada temporalmente debido a varios intentos fallidos.";

        if (!result.Succeeded)
            return "El nombre de usuario o la contraseña son incorrectos.";

        return null;
    }

    public async Task<string?> RegisterAsync(RegisterViewModel vm)
    {
        var user = new ApplicationUser
        {
            FirstName = vm.FirstName.Trim(),
            LastName = vm.LastName.Trim(),
            UserName = vm.UserName.Trim(),
            Email = vm.Email.Trim(),
            PhoneNumber = vm.PhoneNumber.Trim(),
            IsActive = false
        };

        var result = await _userManager.CreateAsync(user, vm.Password);

        if (!result.Succeeded)
            return string.Join(" ", result.Errors.Select(e => e.Description));

        return null;
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task<string?> ForgotPasswordAsync(ForgotPasswordViewModel vm)
    {
        var user = await _userManager.FindByNameAsync(vm.UserName);

        if (user == null)
            return "El usuario no existe.";

        // Aquí posteriormente se enviará el correo con el token.
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        return null;
    }

    public async Task<string?> ResetPasswordAsync(ResetPasswordViewModel vm)
    {
        var user = await _userManager.FindByIdAsync(vm.UserId);

        if (user == null)
            return "El usuario no existe.";

        var result = await _userManager.ResetPasswordAsync(
            user,
            vm.Token,
            vm.NewPassword);

        if (!result.Succeeded)
            return string.Join(" ", result.Errors.Select(e => e.Description));

        return null;
    }
    public async Task<ProfileViewModel?> GetProfileAsync(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);

        if (user == null)
            return null;

        return new ProfileViewModel
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.UserName!,
            Email = user.Email!,
            PhoneNumber = user.PhoneNumber!,
            ProfilePictureUrl = user.ProfilePictureUrl
        };
    }
    public async Task<string?> ConfirmEmailAsync(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return "El enlace de activación no es válido o ya fue utilizado.";

        var result = await _userManager.ConfirmEmailAsync(user, token);

        if (!result.Succeeded)
            return "El enlace de activación no es válido o ya fue utilizado.";

        user.IsActive = true;
        await _userManager.UpdateAsync(user);

        return null;
    }
}