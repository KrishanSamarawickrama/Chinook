using Microsoft.EntityFrameworkCore;

namespace Chinook.Repositories;

public interface IPlaylistRepository : IBaseRepository<Playlist>
{
    bool AddOrRemoveFavorite(string userId, long trackId);
    bool AddOrRemoveTrack(string userId, long trackId, long? playlistId, string? playlistName);
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

    public bool AddOrRemoveFavorite(string userId, long trackId)
    {
        var track = _dbContext.Tracks.SingleOrDefault(t => t.TrackId == trackId);
        if (track == null) return false;

        var userPlayList = _dbContext.Playlists.FirstOrDefault(p =>
            p.UserPlaylists.Any(up => up.UserId == userId && up.Playlist.Name == "My favorite tracks"));

        return AddOrRemoveTrack(userId, trackId, userPlayList?.PlaylistId, "My favorite tracks");
    }

    public bool AddOrRemoveTrack(string userId, long trackId, long? playlistId, string? playlistName)
    {
        var track = _dbContext.Tracks.SingleOrDefault(t => t.TrackId == trackId);
        if (track == null) return false;

        var playlist = _dbContext.UserPlaylists.Include(z => z.Playlist).ThenInclude(y => y.Tracks)
            .SingleOrDefault(x => x.UserId == userId && x.PlaylistId == playlistId)
            ?.Playlist;

        var remove = playlist != null && playlist.Tracks.Contains(track);
        
        if (remove)
        {
            playlist.Tracks.Remove(track);
            _dbSet.Attach(playlist);
            _dbContext.Entry(playlist).State = EntityState.Modified;
        }
        else
        {
            if (playlist == null)
            {
                playlist = new Playlist()
                {
                    Name = playlistName
                };
                
                playlist.UserPlaylists = new List<UserPlaylist>()
                {
                    new UserPlaylist()
                    {
                        UserId = userId
                    }
                };
                playlist.Tracks.Add(track);
                _dbSet.Add(playlist);
            }
            else
            {
                playlist.Tracks.Add(track);
                _dbSet.Attach(playlist);
                _dbContext.Entry(playlist).State = EntityState.Modified;
            }
        }

        _dbContext.SaveChanges();
        return true;
    }
}