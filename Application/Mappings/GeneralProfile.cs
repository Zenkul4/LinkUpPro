using AutoMapper;
using LinkUpProject.Application.ViewModels.Comment;
using LinkUpProject.Application.ViewModels.Post;
using LinkUpProject.Domain.Entities;

namespace LinkUpProject.Application.Mappings;

public class GeneralProfile : Profile
{
    public GeneralProfile()
    {
        CreateMap<Post, SavePostViewModel>()
            .ForMember(dest => dest.ExistingMediaUrl, opt => opt.MapFrom(src => src.MediaUrl))
            .ForMember(dest => dest.YouTubeLink, opt => opt.MapFrom(src => src.ContentType == "Video de YouTube" ? src.MediaUrl : null))
            .ReverseMap()
            .ForMember(dest => dest.MediaUrl, opt => opt.Ignore());

        CreateMap<Post, PostViewModel>()
            .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => $"{src.Author.FirstName} {src.Author.LastName}"))
            .ForMember(dest => dest.AuthorUsername, opt => opt.MapFrom(src => src.Author.UserName))
            .ForMember(dest => dest.AuthorProfilePictureUrl, opt => opt.MapFrom(src => src.Author.ProfilePictureUrl))
            .ForMember(dest => dest.LikesCount, opt => opt.MapFrom(src => src.Reactions.Count(r => r.Type == "Me gusta")))
            .ForMember(dest => dest.DislikesCount, opt => opt.MapFrom(src => src.Reactions.Count(r => r.Type == "No me gusta")))
            .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments.Where(c => c.ParentCommentId == null)));

        CreateMap<Comment, SaveCommentViewModel>().ReverseMap();

        CreateMap<Comment, CommentViewModel>()
            .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => $"{src.Author.FirstName} {src.Author.LastName}"))
            .ForMember(dest => dest.AuthorUsername, opt => opt.MapFrom(src => src.Author.UserName))
            .ForMember(dest => dest.AuthorProfilePictureUrl, opt => opt.MapFrom(src => src.Author.ProfilePictureUrl))
            .ForMember(dest => dest.Replies, opt => opt.MapFrom(src => src.Replies));
    }
}