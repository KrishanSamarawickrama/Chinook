using Microsoft.EntityFrameworkCore;

namespace Chinook.Repositories;

public interface IPlaylistRepository : IBaseRepository<Playlist>
{
    bool AddOrRemoveFavorite(string userId,long trackId);
}

public class PlaylistRepository : BaseRepository<Playlist>, IPlaylistRepository
{
    private readonly ChinookContext _dbContext;
    private readonly DbSet<Playlist> _dbSet;
    
    public PlaylistRepository(IDbContextFactory<ChinookContext> dbFactory) : base(dbFactory)
    {
        _dbContext = dbFactory.CreateDbContext();
        _dbSet = _dbContext.Set<Playlist>();
    }

    public bool AddOrRemoveFavorite(string userId,long trackId)
    {
        var track = _dbContext.Tracks.SingleOrDefault(t => t.TrackId == trackId);
        if (track == null) return false;
       
        var userPlayList = _dbContext.Playlists.FirstOrDefault(p =>
            p.UserPlaylists.Any(up => up.UserId == userId && up.Playlist.Name == "My favorite tracks"));
        
        var remove = userPlayList != null && track != null && userPlayList.Tracks.Contains(track);
        
        if (remove)
        {
            userPlayList.Tracks.Remove(track);
            _dbSet.Attach(userPlayList);
            _dbContext.Entry(userPlayList).State = EntityState.Modified;
        }
        else
        {
            if (userPlayList == null)
            {
               userPlayList = _dbContext.Playlists.SingleOrDefault(p => p.Name == "My favorite tracks");
               userPlayList ??= new Playlist()
                            {
                                Name = "My favorite tracks"
                            };
               userPlayList.UserPlaylists = new List<UserPlaylist>()
               {
                   new UserPlaylist()
                   {
                       PlaylistId = userPlayList.PlaylistId,
                       UserId = userId
                   }
               };
               userPlayList.Tracks.Add(track);
               _dbSet.Add(userPlayList);
            }
            else
            {
                userPlayList.Tracks.Add(track);
                _dbSet.Attach(userPlayList);
                _dbContext.Entry(userPlayList).State = EntityState.Modified;
            }
            
        }
        
        _dbContext.SaveChanges();
        return true;
    }
}