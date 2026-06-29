using LinkUpProject.Application.Interfaces.Services;
using LinkUpProject.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LinkUpProject.Web.Controllers;

//[Authorize]
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
        var userId = "1";
        //var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Account");

        var result = await _postService.GetMyPostsAsync(userId, searchText, contentType, from, to, editState);

        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.ErrorMessage;
            return View(Enumerable.Empty<Application.ViewModels.Post.PostViewModel>());
        }

        ViewBag.SearchText = searchText;
        ViewBag.ContentType = contentType;
        ViewBag.From = from?.ToString("yyyy-MM-dd");
        ViewBag.To = to?.ToString("yyyy-MM-dd");
        ViewBag.EditState = editState;

        return View(result.Data);
    }
}