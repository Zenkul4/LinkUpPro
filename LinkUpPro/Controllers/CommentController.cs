using LinkUpProject.Application.Interfaces.Services;
using LinkUpProject.Application.ViewModels.Comment;
using LinkUpProject.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LinkUpPro.Controllers;

[Authorize]
public class CommentController : Controller
{
    private readonly ICommentService _commentService;
    private readonly UserManager<ApplicationUser> _userManager;

    public CommentController(ICommentService commentService, UserManager<ApplicationUser> userManager)
    {
        _commentService = commentService;
        _userManager = userManager;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SaveCommentViewModel vm)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("Login", "Account");

        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "El contenido del comentario no es valido.";
            return RedirectToAction("Index", "Home");
        }

        var result = await _commentService.CreateCommentAsync(vm, userId);
        if (!result.IsSuccess)
            TempData["ErrorMessage"] = result.ErrorMessage;
        else
            TempData["SuccessMessage"] = "Comentario publicado.";

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("Login", "Account");

        var result = await _commentService.DeleteCommentAsync(id, userId);

        if (!result.IsSuccess)
            TempData["ErrorMessage"] = result.ErrorMessage;

        return RedirectToAction("Index", "Home");
    }
}
