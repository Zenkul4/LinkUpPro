using LinkUpProject.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace LinkUpPro.Controllers;

// [Authorize] // COMENTADO TEMPORALMENTE (BYPASS)
public class ReactionController : Controller
{
    private readonly IReactionService _reactionService;

    public ReactionController(IReactionService reactionService)
    {
        _reactionService = reactionService;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(int postId, string type)
    {
        var userId = "1"; // BYPASS

        var result = await _reactionService.ToggleReactionAsync(postId, userId, type);

        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.ErrorMessage;
        }

        return RedirectToAction("Index", "Home");
    }
}