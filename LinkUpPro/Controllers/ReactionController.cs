using LinkUpProject.Application.Interfaces.Services;
using LinkUpProject.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LinkUpPro.Controllers;

[Authorize]
public class ReactionController : Controller
{
    private readonly IReactionService _reactionService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ReactionController(IReactionService reactionService, UserManager<ApplicationUser> userManager)
    {
        _reactionService = reactionService;
        _userManager = userManager;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(int postId, string type)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("Login", "Account");

        var result = await _reactionService.ToggleReactionAsync(postId, userId, type);

        if (!result.IsSuccess)
            TempData["ErrorMessage"] = result.ErrorMessage;

        return RedirectToAction("Index", "Home");
    }
}
