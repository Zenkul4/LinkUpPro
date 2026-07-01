using Application.DTOs.Email;
using Application.Interfaces.Services;
using LinkUpProject.Application.Interfaces.Services;
using LinkUpProject.Application.ViewModels.Account;
using LinkUpProject.Domain.Common;
using LinkUpProject.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using System.Net;

namespace LinkUpProject.Application.Services;

public class AccountService : IAccountService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IEmailService _emailService;
    private readonly LinkGenerator _linkGenerator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AccountService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IEmailService emailService,
        LinkGenerator linkGenerator,
        IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
        _linkGenerator = linkGenerator;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result> LoginAsync(LoginViewModel vm)
    {
        var user = await _userManager.FindByNameAsync(vm.UserName);

        if (user == null)
            return Result.Failure("El nombre de usuario o la contrasena son incorrectos.");

        if (!user.IsActive || !await _userManager.IsEmailConfirmedAsync(user))
            return Result.Failure("Su cuenta se encuentra inactiva. Debe activarla mediante el enlace enviado a su correo electronico.");

        var result = await _signInManager.PasswordSignInAsync(
            vm.UserName,
            vm.Password,
            vm.RememberMe,
            lockoutOnFailure: true);

        if (result.IsLockedOut)
            return Result.Failure("La cuenta se encuentra bloqueada temporalmente debido a varios intentos fallidos.");

        if (!result.Succeeded)
            return Result.Failure("El nombre de usuario o la contrasena son incorrectos.");

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

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmationLink = BuildActionLink("ConfirmEmail", new { userId = user.Id, token });

        if (string.IsNullOrWhiteSpace(confirmationLink))
            return Result.Failure("La cuenta fue creada, pero no se pudo generar el enlace de activacion.");

        await _emailService.SendAsync(new EmailRequest
        {
            To = user.Email!,
            Subject = "Activa tu cuenta de LinkUpPro",
            Body = BuildConfirmationEmailBody(user.FirstName, confirmationLink)
        });

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
        var resetLink = BuildActionLink("ResetPassword", new { userId = user.Id, token });

        if (string.IsNullOrWhiteSpace(resetLink))
            return Result.Failure("No se pudo generar el enlace para restablecer la contrasena.");

        await _emailService.SendAsync(new EmailRequest
        {
            To = user.Email!,
            Subject = "Restablece tu contrasena de LinkUpPro",
            Body = BuildResetPasswordEmailBody(user.FirstName, resetLink)
        });

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
            return Result.Failure("El enlace de activacion no es valido o ya fue utilizado.");

        var result = await _userManager.ConfirmEmailAsync(user, token);

        if (!result.Succeeded)
            return Result.Failure("El enlace de activacion no es valido o ya fue utilizado.");

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

    private string? BuildActionLink(string action, object values)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext == null)
            return null;

        return _linkGenerator.GetUriByAction(
            httpContext,
            action: action,
            controller: "Account",
            values: values);
    }

    private static string BuildConfirmationEmailBody(string firstName, string confirmationLink)
    {
        var safeName = WebUtility.HtmlEncode(firstName);
        var safeLink = WebUtility.HtmlEncode(confirmationLink);

        return $"""
            <h2>Bienvenido a LinkUpPro, {safeName}</h2>
            <p>Tu cuenta fue creada correctamente. Para activarla, confirma tu correo electronico usando el siguiente enlace:</p>
            <p><a href="{safeLink}">Confirmar mi correo</a></p>
            <p>Si no creaste esta cuenta, puedes ignorar este mensaje.</p>
            """;
    }

    private static string BuildResetPasswordEmailBody(string firstName, string resetLink)
    {
        var safeName = WebUtility.HtmlEncode(firstName);
        var safeLink = WebUtility.HtmlEncode(resetLink);

        return $"""
            <h2>Hola, {safeName}</h2>
            <p>Recibimos una solicitud para restablecer tu contrasena de LinkUpPro.</p>
            <p><a href="{safeLink}">Restablecer mi contrasena</a></p>
            <p>Si no solicitaste este cambio, puedes ignorar este mensaje.</p>
            """;
    }
}
