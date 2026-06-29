using LinkUpProject.Application.Interfaces.Repositories;
using LinkUpProject.Application.Interfaces.Services;
using LinkUpProject.Application.ViewModels.Comment;
using LinkUpProject.Domain.Common;
using LinkUpProject.Domain.Entities;

namespace LinkUpProject.Application.Services;

public class CommentService : ICommentService
{
    private readonly IGenericRepository<Comment> _commentRepository;
    private readonly IGenericRepository<Post> _postRepository;
    private readonly IGenericRepository<Friendship> _friendshipRepository;

    public CommentService(
        IGenericRepository<Comment> commentRepository,
        IGenericRepository<Post> postRepository,
        IGenericRepository<Friendship> friendshipRepository)
    {
        _commentRepository = commentRepository;
        _postRepository = postRepository;
        _friendshipRepository = friendshipRepository;
    }

    public async Task<Result> CreateCommentAsync(SaveCommentViewModel vm, string userId)
    {
        var post = await _postRepository.GetByIdAsync(vm.PostId);
        if (post == null || post.IsDeleted)
            return Result.Failure("La publicación no existe.");

        if (!post.AllowComments)
            return Result.Failure("Esta publicación no admite comentarios.");

        var comment = new Comment
        {
            Content = vm.Content,
            PostId = vm.PostId,
            ParentCommentId = vm.ParentCommentId,
            AuthorId = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _commentRepository.AddAsync(comment);
        return Result.Success();
    }

    public async Task<Result> UpdateCommentAsync(SaveCommentViewModel vm, string userId)
    {
        var comment = await _commentRepository.GetByIdAsync(vm.Id);
        if (comment == null || comment.IsDeleted) return Result.Failure("El comentario no existe.");
        if (comment.AuthorId != userId) return Result.Failure("No posee permisos para editar este contenido.");

        comment.Content = vm.Content;
        comment.LastModifiedAt = DateTime.UtcNow;

        await _commentRepository.UpdateAsync(comment);
        return Result.Success();
    }

    public async Task<Result> DeleteCommentAsync(int commentId, string userId)
    {
        var comment = await _commentRepository.GetByIdAsync(commentId);

        if (comment == null || comment.IsDeleted)
            return Result.Failure("El comentario no existe o ya fue eliminado.");

        if (comment.AuthorId != userId)
            return Result.Failure("No tienes permisos para eliminar este comentario.");

        comment.IsDeleted = true;
        await _commentRepository.UpdateAsync(comment);

        return Result.Success();
    }
}