using LinkUpProject.Application.Interfaces.Services;
using LinkUpProject.Application.ViewModels.Battleship;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LinkUpPro.Controllers;

[Authorize]
public class BattleshipController : Controller
{
    private readonly IBattleshipService _battleshipService;

    public BattleshipController(IBattleshipService battleshipService)
    {
        _battleshipService = battleshipService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _battleshipService.GetIndexAsync(userId);
        return View(result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string opponentId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _battleshipService.CreateMatchAsync(userId, opponentId);

        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.ErrorMessage;
            return RedirectToAction(nameof(Index));
        }

        return RedirectToAction(nameof(Setup), new { id = result.Data });
    }

    [HttpGet]
    public async Task<IActionResult> Setup(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _battleshipService.GetSetupAsync(id, userId);

        if (!result.IsSuccess)
            return RedirectToAction(nameof(Index));

        return View(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> SaveShips(BattleshipSetupViewModel vm)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var result = await _battleshipService.SaveShipsAsync(vm, userId);

        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.ErrorMessage;
            return RedirectToAction(nameof(Setup), new { id = vm.MatchId });
        }

        return RedirectToAction(nameof(Match), new { id = vm.MatchId });
    }


    [HttpGet]
    public async Task<IActionResult> Match(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _battleshipService.GetGameplayAsync(id, userId);

        if (!result.IsSuccess)
            return RedirectToAction(nameof(Index));

        return View(result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Attack(int matchId, int x, int y)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _battleshipService.AttackAsync(matchId, userId, x, y);

        if (!result.IsSuccess)
            return Json(new { success = false, message = result.ErrorMessage });

        return Json(new { success = true });
    }

    [HttpGet]
    public async Task<IActionResult> MatchStatus(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _battleshipService.GetGameplayAsync(id, userId);

        if (!result.IsSuccess)
            return Json(new { success = false });

        return Json(new
        {
            success = true,
            isMyTurn = result.Data.IsMyTurn,
            status = result.Data.Status,
            attacksCount = result.Data.MyAttacks.Count + result.Data.OpponentAttacks.Count
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _battleshipService.CancelMatchAsync(id, userId);

        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.ErrorMessage;
        }

        return RedirectToAction(nameof(Index));
    }
}