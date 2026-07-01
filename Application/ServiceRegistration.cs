
using LinkUpProject.Application.Interfaces.Services;
using LinkUpProject.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace LinkUpProject.Application;

public static class ServiceRegistration
{
    public static void AddApplicationLayer(this IServiceCollection services)
    {
        services.AddAutoMapper(config => config.AddMaps(Assembly.GetExecutingAssembly()));

        services.AddTransient<IPostService, PostService>();
        services.AddTransient<ICommentService, CommentService>();
        services.AddTransient<IStorageService, StorageService>();
        services.AddTransient<IAccountService, AccountService>();
        services.AddTransient<IReactionService, ReactionService>();
        services.AddTransient<IFriendService, FriendService>();
    }
}
