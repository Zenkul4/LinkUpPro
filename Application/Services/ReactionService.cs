using LinkUpProject.Application.Interfaces.Repositories;
using LinkUpProject.Application.Interfaces.Services;
using LinkUpProject.Domain.Common;
using LinkUpProject.Domain.Entities;

namespace LinkUpProject.Application.Services;

public class ReactionService : IReactionService
{
    private readonly IGenericRepository<Reaction> _reactionRepository;
    private readonly IGenericRepository<Post> _postRepository;

    public ReactionService(IGenericRepository<Reaction> reactionRepository, IGenericRepository<Post> postRepository)
    {
        _reactionRepository = reactionRepository;
        _postRepository = postRepository;
    }

    public async Task<Result> ToggleReactionAsync(int postId, string userId, string reactionType)
    {
        if (reactionType != "Me gusta" && reactionType != "No me gusta")
            return Result.Failure("Tipo de reacción inválida.");

        var post = await _postRepository.GetByIdAsync(postId);
        if (post == null || post.IsDeleted)
            return Result.Failure("La publicación no existe.");

        var existingReactions = await _reactionRepository.FindAsync(r => r.PostId == postId && r.UserId == userId);
        var currentReaction = existingReactions.FirstOrDefault();

        if (currentReaction != null)
        {
            if (currentReaction.Type == reactionType)
            {
                // Si presiona el mismo botón, se elimina la reacción (Toggle Off)
                await _reactionRepository.DeleteAsync(currentReaction);
            }
            else
            {
                // Si presiona el botón contrario, se actualiza la reacción
                currentReaction.Type = reactionType;
                await _reactionRepository.UpdateAsync(currentReaction);
            }
        }
        else
        {
            // Si no tiene reacción, se crea una nueva
            var newReaction = new Reaction
            {
                PostId = postId,
                UserId = userId,
                Type = reactionType
            };
            await _reactionRepository.AddAsync(newReaction);
        }

        return Result.Success();
    }
}