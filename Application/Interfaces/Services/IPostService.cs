using LinkUpProject.Application.ViewModels.Post;
using LinkUpProject.Domain.Common;

namespace LinkUpProject.Application.Interfaces.Services;

public interface IPostService
{
    Task<Result> CreatePostAsync(SavePostViewModel vm, string userId);
    Task<Result> UpdatePostAsync(SavePostViewModel vm, string userId);
    Task<Result> DeletePostAsync(int postId, string userId);
    Task<Result<SavePostViewModel>> GetPostForEditAsync(int postId, string userId);
    Task<Result<IEnumerable<PostViewModel>>> GetMyPostsAsync(string userId, string? searchText, string? contentType, DateTime? from, DateTime? to, string? editState);
    //Task<Result<IEnumerable<PostViewModel>>> GetFeedAsync(string userId, string? searchText, string? contentType, DateTime? from, DateTime? to, string? editState);
}