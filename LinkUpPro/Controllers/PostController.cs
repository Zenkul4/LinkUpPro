using LinkUpProject.Application.Interfaces.Services;
using LinkUpProject.Application.ViewModels.Post;
using Microsoft.AspNetCore.Mvc;

namespace LinkUpPro.Controllers;

// [Authorize] // COMENTADO TEMPORALMENTE (BYPASS)
public class PostController : Controller
{
    private readonly IPostService _postService;

    public PostController(IPostService postService)
    {
        _postService = postService;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SavePostViewModel vm)
    {
        var userId = "1"; // BYPASS

        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Los datos de la publicación no son válidos.";
            return RedirectToAction("Index", "Home");
        }

        var result = await _postService.CreatePostAsync(vm, userId);

        if (!result.IsSuccess)
            TempData["ErrorMessage"] = result.ErrorMessage;
        else
            TempData["SuccessMessage"] = "Publicación creada correctamente.";

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var userId = "1"; // BYPASS

        // AHORA SÍ PASAMOS EL userId COMO SEGUNDO PARÁMETRO
        var result = await _postService.GetPostForEditAsync(id, userId);

        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = "No se pudo cargar la publicación, no existe o no tienes permisos.";
            return RedirectToAction("Index", "Home");
        }

        var post = result.Data;

        return View(post);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(SavePostViewModel vm)
    {
        var userId = "1"; // BYPASS

        if (!ModelState.IsValid) return View(vm);

        var result = await _postService.UpdatePostAsync(vm, userId);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View(vm);
        }

        TempData["SuccessMessage"] = "Publicación actualizada correctamente.";
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = "1"; // BYPASS

        var result = await _postService.DeletePostAsync(id, userId);

        if (!result.IsSuccess)
            TempData["ErrorMessage"] = result.ErrorMessage;
        else
            TempData["SuccessMessage"] = "Publicación eliminada.";

        return RedirectToAction("Index", "Home");
    }
}