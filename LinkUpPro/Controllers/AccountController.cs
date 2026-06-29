using LinkUpProject.Application.ViewModels.Account;
using LinkUpProject.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

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

        TempData["Success"] = "Su cuenta fue creada correctamente. Hemos enviado un enlace de activación a su correo electrónico.";
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

        TempData["Success"] = "Si el nombre de usuario corresponde a una cuenta registrada, recibirá un enlace para restablecer su contraseña.";
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

        TempData["Success"] = "Su contraseña fue restablecida correctamente. Ya puede iniciar sesión.";
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
    public async Task<IActionResult> Profile()
    {
        if (!User.Identity!.IsAuthenticated)
            return RedirectToAction(nameof(Login));

        var result = await _accountService.GetProfileAsync(User.Identity.Name!);

        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.ErrorMessage;
            return RedirectToAction("Index", "Home");
        }

        return View(result.Data);
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

        TempData["Success"] = "Su cuenta fue activada correctamente. Ya puede iniciar sesión.";
        return RedirectToAction(nameof(Login));
    }
}