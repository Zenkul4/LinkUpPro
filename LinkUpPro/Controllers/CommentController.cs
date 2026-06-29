using LinkUpProject.Application.Interfaces.Services;
using LinkUpProject.Application.ViewModels.Comment;
using Microsoft.AspNetCore.Mvc;

namespace LinkUpPro.Controllers;

// [Authorize] // <-- COMENTADO TEMPORALMENTE (BYPASS)
public class CommentController : Controller
{
    private readonly ICommentService _commentService;

    public CommentController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SaveCommentViewModel vm)
    {
        // BYPASS: Forzamos el ID de tu usuario dummy
        var userId = "1";

        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "El contenido del comentario no es válido.";
            return RedirectToAction("Index", "Home");
        }

        var result = await _commentService.CreateCommentAsync(vm, userId);
        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.ErrorMessage;
        }
        else
        {
            TempData["SuccessMessage"] = "Comentario publicado.";
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = "1"; // BYPASS

        var result = await _commentService.DeleteCommentAsync(id, userId);

        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.ErrorMessage;
        }

        return RedirectToAction("Index", "Home");
    }
}