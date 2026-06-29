using LinkUpProject.Domain.Common;

namespace LinkUpProject.Application.Interfaces.Services;

public interface IReactionService
{
    Task<Result> ToggleReactionAsync(int postId, string userId, string reactionType);
}