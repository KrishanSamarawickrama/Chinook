
namespace Chinook.Extensions;

public static class ServiceExtension
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddTransient(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddTransient<IPlaylistRepository,PlaylistRepository>();
        services.AddTransient<IUserPlaylistRepository,UserPlaylistRepository>();
        return services;
    }
}