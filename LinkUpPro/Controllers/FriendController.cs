using LinkUpProject.Application.Interfaces.Services;
using LinkUpProject.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LinkUpPro.Controllers;

[Authorize]
public class FriendController : Controller
{
    private readonly IFriendService _friendService;
    private readonly UserManager<ApplicationUser> _userManager;

    public FriendController(IFriendService friendService, UserManager<ApplicationUser> userManager)
    {
        _friendService = friendService;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? searchTerm)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrWhiteSpace(userId))
            return RedirectToAction("Login", "Account");

        var result = await _friendService.GetDashboardAsync(userId, searchTerm);
        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.ErrorMessage;
            return RedirectToAction("Index", "Home");
        }

        return View(result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendRequest(string receiverId)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrWhiteSpace(userId))
            return RedirectToAction("Login", "Account");

        var result = await _friendService.SendRequestAsync(userId, receiverId);
        SetMessage(result, "Solicitud enviada correctamente.");

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Accept(int requestId)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrWhiteSpace(userId))
            return RedirectToAction("Login", "Account");

        var result = await _friendService.AcceptRequestAsync(userId, requestId);
        SetMessage(result, "Solicitud aceptada correctamente.");

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(int requestId)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrWhiteSpace(userId))
            return RedirectToAction("Login", "Account");

        var result = await _friendService.RejectRequestAsync(userId, requestId);
        SetMessage(result, "Solicitud rechazada.");

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int requestId)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrWhiteSpace(userId))
            return RedirectToAction("Login", "Account");

        var result = await _friendService.CancelRequestAsync(userId, requestId);
        SetMessage(result, "Solicitud cancelada.");

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(string friendId)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrWhiteSpace(userId))
            return RedirectToAction("Login", "Account");

        var result = await _friendService.RemoveFriendAsync(userId, friendId);
        SetMessage(result, "Amistad eliminada.");

        return RedirectToAction(nameof(Index));
    }

    private void SetMessage(LinkUpProject.Domain.Common.Result result, string successMessage)
    {
        if (result.IsSuccess)
            TempData["SuccessMessage"] = successMessage;
        else
            TempData["ErrorMessage"] = result.ErrorMessage;
    }
}
