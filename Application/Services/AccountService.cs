using LinkUpProject.Application.Interfaces.Services;
using LinkUpProject.Application.ViewModels.Account;
using LinkUpProject.Domain.Common;
using LinkUpProject.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace LinkUpProject.Application.Services;

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

    public async Task<Result> LoginAsync(LoginViewModel vm)
    {
        var user = await _userManager.FindByNameAsync(vm.UserName);

        if (user == null)
            return Result.Failure("El nombre de usuario o la contraseña son incorrectos.");

        if (!user.IsActive || !await _userManager.IsEmailConfirmedAsync(user))
            return Result.Failure("Su cuenta se encuentra inactiva. Debe activarla mediante el enlace enviado a su correo electrónico.");

        var result = await _signInManager.PasswordSignInAsync(
            vm.UserName,
            vm.Password,
            vm.RememberMe,
            lockoutOnFailure: true);

        if (result.IsLockedOut)
            return Result.Failure("La cuenta se encuentra bloqueada temporalmente debido a varios intentos fallidos.");

        if (!result.Succeeded)
            return Result.Failure("El nombre de usuario o la contraseña son incorrectos.");

        return Result.Success();
    }

    public async Task<Result> RegisterAsync(RegisterViewModel vm)
    {
        var user = new ApplicationUser
        {
            FirstName = vm.FirstName.Trim(),
            LastName = vm.LastName.Trim(),
            UserName = vm.UserName.Trim(),
            Email = vm.Email.Trim(),
            PhoneNumber = vm.PhoneNumber?.Trim(),
            IsActive = false
        };

        var result = await _userManager.CreateAsync(user, vm.Password);

        if (!result.Succeeded)
            return Result.Failure(string.Join(" ", result.Errors.Select(e => e.Description)));

        return Result.Success();
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task<Result> ForgotPasswordAsync(ForgotPasswordViewModel vm)
    {
        var user = await _userManager.FindByNameAsync(vm.UserName);

        if (user == null)
            return Result.Failure("El usuario no existe.");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        // TODO: Lógica de envío de correo para el Sprint 4

        return Result.Success();
    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordViewModel vm)
    {
        var user = await _userManager.FindByIdAsync(vm.UserId);

        if (user == null)
            return Result.Failure("El usuario no existe.");

        var result = await _userManager.ResetPasswordAsync(
            user,
            vm.Token,
            vm.NewPassword);

        if (!result.Succeeded)
            return Result.Failure(string.Join(" ", result.Errors.Select(e => e.Description)));

        return Result.Success();
    }

    public async Task<Result<ProfileViewModel>> GetProfileAsync(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);

        if (user == null)
            return Result<ProfileViewModel>.Failure("Usuario no encontrado.");

        var profile = new ProfileViewModel
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.UserName!,
            Email = user.Email!,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            ProfilePictureUrl = user.ProfilePictureUrl
        };

        return Result<ProfileViewModel>.Success(profile);
    }

    public async Task<Result> ConfirmEmailAsync(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return Result.Failure("El enlace de activación no es válido o ya fue utilizado.");

        var result = await _userManager.ConfirmEmailAsync(user, token);

        if (!result.Succeeded)
            return Result.Failure("El enlace de activación no es válido o ya fue utilizado.");

        user.IsActive = true;
        await _userManager.UpdateAsync(user);

        return Result.Success();
    }

    public async Task<Result> UpdateProfileAsync(ProfileViewModel vm, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return Result.Failure("Usuario no encontrado.");

        user.FirstName = vm.FirstName;
        user.LastName = vm.LastName;
        user.PhoneNumber = vm.PhoneNumber;

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded) return Result.Success();

        return Result.Failure("Error al actualizar el perfil.");
    }
}