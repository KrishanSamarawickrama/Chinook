using Microsoft.EntityFrameworkCore;

namespace Chinook.Repositories;

public interface IUserPlaylistRepository : IBaseRepository<UserPlaylist>
{
    void Delete(string userId, long playlistId);
}

public class UserPlaylistRepository : BaseRepository<UserPlaylist>, IUserPlaylistRepository
{
    public UserPlaylistRepository(IDbContextFactory<ChinookContext> dbFactory) : base(dbFactory)
    {
    }

    public void Delete(string userId, long playlistId)
    {
        var userPlaylist = _dbContext.UserPlaylists.SingleOrDefault(x => x.PlaylistId == playlistId && x.UserId == userId);
        if( userPlaylist != null)
            base.Delete(userPlaylist);
    }
}