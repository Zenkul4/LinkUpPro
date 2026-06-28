using Application.Interfaces.Services;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application;

public static class ServiceRegistration
{
    public static void AddApplicationLayer(this IServiceCollection services)
    {
        services.AddAutoMapper(config => config.AddMaps(Assembly.GetExecutingAssembly()));

        services.AddScoped<IAccountService, AccountService>();
    }
}