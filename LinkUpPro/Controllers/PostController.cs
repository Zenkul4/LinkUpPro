using LinkUpProject.Application.Interfaces.Services;
using LinkUpProject.Application.ViewModels.Post;
using LinkUpProject.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LinkUpPro.Controllers;

[Authorize]
public class PostController : Controller
{
    private readonly IPostService _postService;
    private readonly UserManager<ApplicationUser> _userManager;

    public PostController(IPostService postService, UserManager<ApplicationUser> userManager)
    {
        _postService = postService;
        _userManager = userManager;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SavePostViewModel vm)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("Login", "Account");

        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Los datos de la publicacion no son validos.";
            return RedirectToAction("Index", "Home");
        }

        var result = await _postService.CreatePostAsync(vm, userId);

        if (!result.IsSuccess)
            TempData["ErrorMessage"] = result.ErrorMessage;
        else
            TempData["SuccessMessage"] = "Publicacion creada correctamente.";

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("Login", "Account");

        var result = await _postService.GetPostForEditAsync(id, userId);

        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = "No se pudo cargar la publicacion, no existe o no tienes permisos.";
            return RedirectToAction("Index", "Home");
        }

        return View(result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(SavePostViewModel vm)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("Login", "Account");

        if (!ModelState.IsValid) return View(vm);

        var result = await _postService.UpdatePostAsync(vm, userId);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View(vm);
        }

        TempData["SuccessMessage"] = "Publicacion actualizada correctamente.";
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("Login", "Account");

        var result = await _postService.DeletePostAsync(id, userId);

        if (!result.IsSuccess)
            TempData["ErrorMessage"] = result.ErrorMessage;
        else
            TempData["SuccessMessage"] = "Publicacion eliminada.";

        return RedirectToAction("Index", "Home");
    }
}
