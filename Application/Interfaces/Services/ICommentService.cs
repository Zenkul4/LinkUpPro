using LinkUpProject.Application.ViewModels.Comment;
using LinkUpProject.Domain.Common;

namespace LinkUpProject.Application.Interfaces.Services;

public interface ICommentService
{
    Task<Result> CreateCommentAsync(SaveCommentViewModel vm, string userId);
    Task<Result> UpdateCommentAsync(SaveCommentViewModel vm, string userId);
    Task<Result> DeleteCommentAsync(int commentId, string userId);
}