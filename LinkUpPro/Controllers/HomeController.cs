using LinkUpProject.Application.Interfaces.Services;
using LinkUpProject.Application.ViewModels.Post;
using LinkUpProject.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LinkUpPro.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly IPostService _postService;
    private readonly UserManager<ApplicationUser> _userManager;

    public HomeController(IPostService postService, UserManager<ApplicationUser> userManager)
    {
        _postService = postService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(string? searchText, string? contentType, DateTime? from, DateTime? to, string? editState)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("Login", "Account");
        if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Account");
        if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Account");
        if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Account");
        if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Account");
        if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Account");

        var result = await _postService.GetFeedAsync(userId, searchText, contentType, from, to, editState);

        ViewBag.SearchText = searchText;
        ViewBag.ContentType = contentType ?? "Todos";
        ViewBag.From = from?.ToString("yyyy-MM-dd");
        ViewBag.To = to?.ToString("yyyy-MM-dd");
            return View(Enumerable.Empty<PostViewModel>());
            return View(Enumerable.Empty<Application.ViewModels.Post.PostViewModel>());
            return View(Enumerable.Empty<Application.ViewModels.Post.PostViewModel>());
            return View(Enumerable.Empty<Application.ViewModels.Post.PostViewModel>());
            return View(Enumerable.Empty<Application.ViewModels.Post.PostViewModel>());
            return View(Enumerable.Empty<Application.ViewModels.Post.PostViewModel>());
        }

        return View(result.Data);
    }
}
