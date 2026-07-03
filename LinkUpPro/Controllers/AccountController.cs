using LinkUpProject.Application.Interfaces.Services;
using LinkUpProject.Application.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LinkUpPro.Controllers;

public class AccountController : Controller
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var result = await _accountService.LoginAsync(vm);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View(vm);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var result = await _accountService.RegisterAsync(vm);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View(vm);
        }

        TempData["Success"] = "Su cuenta fue creada correctamente. Hemos enviado un enlace de activacion a su correo electronico.";
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult ForgotPassword() => View();

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var result = await _accountService.ForgotPasswordAsync(vm);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View(vm);
        }

        TempData["Success"] = "Si el nombre de usuario corresponde a una cuenta registrada, recibira un enlace para restablecer su contrasena.";
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult ResetPassword(string userId, string token)
    {
        return View(new ResetPasswordViewModel
        {
            UserId = userId,
            Token = token
        });
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var result = await _accountService.ResetPasswordAsync(vm);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View(vm);
        }

        TempData["Success"] = "Su contrasena fue restablecida correctamente. Ya puede iniciar sesion.";
        return RedirectToAction(nameof(Login));
    }

    public IActionResult AccessDenied() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _accountService.LogoutAsync();
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var result = await _accountService.GetProfileAsync(User.Identity!.Name!);

        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.ErrorMessage;
            return RedirectToAction("Index", "Home");
        }

        return View(result.Data);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> EditProfile()
    {
        var result = await _accountService.GetProfileAsync(User.Identity!.Name!);

        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.ErrorMessage;
            return RedirectToAction("Index", "Home");
        }

        result.Data!.IsEditing = true;
        return View("Profile", result.Data);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProfile(ProfileViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            vm.IsEditing = true;
            return View("Profile", vm);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
            return RedirectToAction(nameof(Login));

        var result = await _accountService.UpdateProfileAsync(vm, userId);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            vm.IsEditing = true;
            return View("Profile", vm);
        }

        TempData["Success"] = "Tu informacion de perfil fue actualizada correctamente.";
        return RedirectToAction(nameof(Profile));
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        var result = await _accountService.ConfirmEmailAsync(userId, token);

        if (!result.IsSuccess)
        {
            TempData["Error"] = result.ErrorMessage;
            return RedirectToAction(nameof(Login));
        }

        TempData["Success"] = "Su cuenta fue activada correctamente. Ya puede iniciar sesion.";
        return RedirectToAction(nameof(Login));
    }
}
