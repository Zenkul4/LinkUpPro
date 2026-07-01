using AutoMapper;
using LinkUpProject.Application.Interfaces.Repositories;
using LinkUpProject.Application.Interfaces.Services;
using LinkUpProject.Application.ViewModels.Post;
using LinkUpProject.Domain.Common;
using LinkUpProject.Domain.Entities;
using System.Web;

namespace LinkUpProject.Application.Services;

public class PostService : IPostService
{
    private readonly IGenericRepository<Post> _postRepository;
    private readonly IFriendRepository _friendRepository; 
    private readonly IStorageService _storageService;
    private readonly IMapper _mapper;

    public PostService(
        IGenericRepository<Post> postRepository,
        IFriendRepository friendRepository, 
        IStorageService storageService,
        IMapper mapper)
    {
        _postRepository = postRepository;
        _friendRepository = friendRepository;
        _storageService = storageService;
        _mapper = mapper;
    }

    public async Task<Result> CreatePostAsync(SavePostViewModel vm, string userId)
    {
        if (vm.Privacy != "Solo amigos" && vm.Privacy != "Solo yo")
            return Result.Failure("La privacidad seleccionada no es válida.");

        if (vm.ContentType == "Imagen" && vm.MediaFile == null)
            return Result.Failure("Debe seleccionar una imagen para crear la publicación.");

        if (vm.ContentType == "Video de YouTube" && string.IsNullOrWhiteSpace(vm.YouTubeLink))
            return Result.Failure("Debe ingresar un enlace válido de YouTube.");

        if (vm.ContentType == "Imagen" && !string.IsNullOrWhiteSpace(vm.YouTubeLink))
            return Result.Failure("No puede enviar simultáneamente una imagen y un enlace de YouTube.");

        var post = _mapper.Map<Post>(vm);
        post.AuthorId = userId;
        post.CreatedAt = DateTime.UtcNow;

        if (vm.ContentType == "Imagen")
        {
            post.MediaUrl = await _storageService.UploadFileAsync(vm.MediaFile!, "posts");
        }
        else if (vm.ContentType == "Video de YouTube")
        {
            post.MediaUrl = ExtractYouTubeEmbedUrl(vm.YouTubeLink!);
        }

        await _postRepository.AddAsync(post);
        return Result.Success();
    }

    public async Task<Result<SavePostViewModel>> GetPostForEditAsync(int postId, string userId)
    {
        var post = await _postRepository.GetByIdAsync(postId);
        if (post == null || post.IsDeleted) return Result<SavePostViewModel>.Failure("Publicación no encontrada.");
        if (post.AuthorId != userId) return Result<SavePostViewModel>.Failure("No posee permisos para editar esta publicación.");

        var vm = _mapper.Map<SavePostViewModel>(post);
        return Result<SavePostViewModel>.Success(vm);
    }

    public async Task<Result> UpdatePostAsync(SavePostViewModel vm, string userId)
    {
        var post = await _postRepository.GetByIdAsync(vm.Id);
        if (post == null || post.IsDeleted) return Result.Failure("Publicación no encontrada.");
        if (post.AuthorId != userId) return Result.Failure("No posee permisos para editar esta publicación.");

        if (vm.ContentType == "Imagen" && vm.MediaFile == null && string.IsNullOrWhiteSpace(post.MediaUrl) && post.ContentType != "Imagen")
            return Result.Failure("Debe seleccionar una imagen para crear la publicación.");

        if (vm.ContentType == "Video de YouTube" && string.IsNullOrWhiteSpace(vm.YouTubeLink))
            return Result.Failure("Debe ingresar un enlace válido de YouTube.");

        post.Content = vm.Content;
        post.Privacy = vm.Privacy;
        post.AllowComments = vm.AllowComments;
        post.LastModifiedAt = DateTime.UtcNow;

        if (vm.ContentType != post.ContentType)
        {
            if (post.ContentType == "Imagen" && !string.IsNullOrEmpty(post.MediaUrl))
            {
                _storageService.DeleteFile(post.MediaUrl);
            }
            post.MediaUrl = string.Empty;
        }

        post.ContentType = vm.ContentType;

        if (vm.ContentType == "Imagen" && vm.MediaFile != null)
        {
            if (!string.IsNullOrEmpty(post.MediaUrl)) _storageService.DeleteFile(post.MediaUrl);
            post.MediaUrl = await _storageService.UploadFileAsync(vm.MediaFile, "posts");
        }
        else if (vm.ContentType == "Video de YouTube")
        {
            post.MediaUrl = ExtractYouTubeEmbedUrl(vm.YouTubeLink!);
        }

        await _postRepository.UpdateAsync(post);
        return Result.Success();
    }

    public async Task<Result> DeletePostAsync(int postId, string userId)
    {
        var post = await _postRepository.GetByIdAsync(postId);
        if (post == null || post.IsDeleted) return Result.Failure("Publicación no encontrada.");
        if (post.AuthorId != userId) return Result.Failure("No posee permisos para eliminar esta publicación.");

        post.IsDeleted = true;
        await _postRepository.UpdateAsync(post);

        return Result.Success();
    }

    public async Task<Result<IEnumerable<PostViewModel>>> GetFeedAsync(string userId, string? searchText, string? contentType, DateTime? from, DateTime? to, string? editState)
    {
        if (from.HasValue && to.HasValue && from.Value > to.Value)
            return Result<IEnumerable<PostViewModel>>.Failure("La fecha inicial no puede ser posterior a la fecha final.");

        var friendships = await _friendRepository.GetActiveFriendshipsAsync(userId);

        var friendIds = friendships.Select(f => f.User1Id == userId ? f.User2Id : f.User1Id).ToList();

        var includes = new List<string> { "Author", "Reactions", "Comments", "Comments.Author", "Comments.Replies", "Comments.Replies.Author" };

        IEnumerable<Post> allPosts = await _postRepository.FindWithIncludeAsync(p => !p.IsDeleted, includes);

        var posts = allPosts.Where(p =>
            p.AuthorId == userId ||
            (friendIds.Contains(p.AuthorId) && p.Privacy == "Solo amigos")
        ).ToList();

        foreach (var post in posts)
        {
            post.Comments = post.Comments.Where(c => !c.IsDeleted).ToList();
            foreach (var comment in post.Comments)
            {
                comment.Replies = comment.Replies.Where(r => !r.IsDeleted).ToList();
            }
        }

        if (!string.IsNullOrWhiteSpace(searchText))
            posts = posts.Where(p => p.Content.Contains(searchText.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();

        if (!string.IsNullOrWhiteSpace(contentType) && contentType != "Todos")
            posts = posts.Where(p => p.ContentType == contentType).ToList();

        if (from.HasValue)
            posts = posts.Where(p => p.CreatedAt.Date >= from.Value.Date).ToList();

        if (to.HasValue)
            posts = posts.Where(p => p.CreatedAt.Date <= to.Value.Date).ToList();

        if (!string.IsNullOrWhiteSpace(editState) && editState != "Todas")
        {
            if (editState == "Editadas") posts = posts.Where(p => p.LastModifiedAt.HasValue).ToList();
            else if (editState == "No editadas") posts = posts.Where(p => !p.LastModifiedAt.HasValue).ToList();
        }

        var orderedPosts = posts.OrderByDescending(p => p.CreatedAt).ToList();
        var viewModels = _mapper.Map<IEnumerable<PostViewModel>>(orderedPosts).ToList();

        foreach (var vm in viewModels)
        {
            var post = orderedPosts.First(p => p.Id == vm.Id);
            var userReaction = post.Reactions.FirstOrDefault(r => r.UserId == userId);
            vm.CurrentUserReaction = userReaction?.Type;
        }

        return Result<IEnumerable<PostViewModel>>.Success(viewModels);
    }

    private string ExtractYouTubeEmbedUrl(string url)
    {
        try
        {
            var uri = new Uri(url);
            var query = HttpUtility.ParseQueryString(uri.Query);
            var videoId = string.Empty;

            if (query.AllKeys.Contains("v"))
            {
                videoId = query["v"];
            }
            else
            {
                videoId = uri.Segments.Last();
            }

            return $"https://www.youtube.com/embed/{videoId}";
        }
        catch
        {
            return url;
        }
    }
}