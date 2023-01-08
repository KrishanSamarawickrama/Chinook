
namespace Chinook.Extensions;

public static class ServiceExtension
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddTransient(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        return services;
    }
}