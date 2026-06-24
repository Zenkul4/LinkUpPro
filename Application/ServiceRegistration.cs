using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application;

public static class ServiceRegistration
{
    public static void AddApplicationLayer(this IServiceCollection services)
    {
        services.AddAutoMapper(config => config.AddMaps(Assembly.GetExecutingAssembly()));

        // Aquí registrarás tus GenericServices y Services específicos más adelante
    }
}