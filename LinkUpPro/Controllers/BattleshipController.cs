using LinkUpProject.Application.Interfaces.Services;
using LinkUpProject.Application.ViewModels.Battleship;
using LinkUpProject.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LinkUpPro.Controllers;

[Authorize]
public class BattleshipController : Controller
{
    private readonly IBattleshipService _battleshipService;
    private readonly UserManager<ApplicationUser> _userManager;

    public BattleshipController(IBattleshipService battleshipService, UserManager<ApplicationUser> userManager)
    {
        _battleshipService = battleshipService;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrWhiteSpace(userId))
            return RedirectToAction("Login", "Account");

        var result = await _battleshipService.GetIndexAsync(userId);
        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.ErrorMessage;
            return RedirectToAction("Index", "Home");
        }

        return View(result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string opponentId)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrWhiteSpace(userId))
            return RedirectToAction("Login", "Account");

        var result = await _battleshipService.CreateMatchAsync(userId, opponentId);
        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.ErrorMessage;
            return RedirectToAction(nameof(Index));
        }

        TempData["SuccessMessage"] = "Partida creada. Coloca tus barcos para continuar.";
        return RedirectToAction(nameof(Setup), new { id = result.Data });
    }

    [HttpGet]
    public async Task<IActionResult> Setup(int id)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrWhiteSpace(userId))
            return RedirectToAction("Login", "Account");

        var result = await _battleshipService.GetSetupAsync(id, userId);
        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.ErrorMessage;
            return RedirectToAction(nameof(Index));
        }

        return View(result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Setup(BattleshipSetupViewModel viewModel)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrWhiteSpace(userId))
            return RedirectToAction("Login", "Account");

        if (!ModelState.IsValid)
            return View(viewModel);

        var result = await _battleshipService.SaveShipsAsync(viewModel, userId);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View(viewModel);
        }

        TempData["SuccessMessage"] = "Barcos guardados correctamente.";
        return RedirectToAction(nameof(Setup), new { id = viewModel.MatchId });
    }
}
