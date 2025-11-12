using IntelligentMusicRecommender.Data;
using IntelligentMusicRecommender.Models;
using IntelligentMusicRecommender.Services.Api;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IntelligentMusicRecommender.Services
{
    // 1) ДОБАВЕН е DeleteAsync в интерфейса
    public interface IPlaylistService
    {
        Task<Playlist> CreateRecommendedAsync(string? mood, string? genre, int limit = 15, CancellationToken ct = default);
        Task<Playlist?> GetPlaylistAsync(int id, CancellationToken ct = default);
        Task<IReadOnlyList<Playlist>> GetAllAsync(CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);   // <-- добавено
    }

    public class PlaylistService : IPlaylistService
    {
        private readonly AppDbContext _db;
        private readonly IMockAiService _ai;
        private readonly IMusicApiClient _api;
        private readonly ILogger<PlaylistService> _logger;

        public PlaylistService(AppDbContext db, IMockAiService ai, IMusicApiClient api, ILogger<PlaylistService> logger)
        {
            _db = db; _ai = ai; _api = api; _logger = logger;
        }

        public async Task<Playlist> CreateRecommendedAsync(string? mood, string? genre, int limit = 15, CancellationToken ct = default)
        {
            try
            {
                _logger.LogInformation("CreateRecommendedAsync started. mood={Mood}, genre={Genre}, limit={Limit}", mood, genre, limit);

                var req = new AiRecommendationRequest(mood, genre, limit);

                var seed = await _ai.BuildSeedAsync(req, ct);
                var candidates = await _api.SearchTracksAsync(seed.OrderByDescending(x => x.Value).Select(x => x.Key), limit * 3, ct);
                var ranked = await _ai.RankAsync(candidates, req, ct);
                var final = ranked.Take(limit).ToList();

                // persist tracks
                var persisted = new List<Track>();
                foreach (var t in final)
                {
                    var existing = await _db.Tracks.FirstOrDefaultAsync(x => x.ExternalId == t.ExternalId && x.Source == t.Source, ct);
                    if (existing is null) { _db.Tracks.Add(t); persisted.Add(t); }
                    else persisted.Add(existing);
                }
                await _db.SaveChangesAsync(ct);

                // create playlist
                var playlist = new Playlist
                {
                    Name = $"AI Playlist ({mood ?? genre ?? "default"})",
                    Criteria = $"mood={mood ?? "-"}, genre={genre ?? "-"}"
                };
                _db.Playlists.Add(playlist);
                await _db.SaveChangesAsync(ct);

                int order = 1;
                foreach (var t in persisted)
                    _db.PlaylistTracks.Add(new PlaylistTrack { PlaylistId = playlist.Id, TrackId = t.Id, Order = order++ });

                await _db.SaveChangesAsync(ct);

                _logger.LogInformation("CreateRecommendedAsync succeeded. playlistId={PlaylistId}, tracks={Count}", playlist.Id, order - 1);
                return playlist;
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("CreateRecommendedAsync canceled by caller.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateRecommendedAsync failed. mood={Mood}, genre={Genre}, limit={Limit}", mood, genre, limit);
                throw;
            }
        }

        public Task<Playlist?> GetPlaylistAsync(int id, CancellationToken ct = default)
            => _db.Playlists
                  .Include(p => p.Tracks)
                  .ThenInclude(pt => pt.Track)
                  .FirstOrDefaultAsync(p => p.Id == id, ct);

        public async Task<IReadOnlyList<Playlist>> GetAllAsync(CancellationToken ct = default)
            => await _db.Playlists
                        .OrderByDescending(p => p.CreatedAt)
                        .ToListAsync(ct);

        // 2) ИМПЛЕМЕНТАЦИЯ на DeleteAsync
        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var pl = await _db.Playlists.FirstOrDefaultAsync(p => p.Id == id, ct);
            if (pl is null)
            {
                _logger.LogWarning("DeleteAsync: playlist {Id} not found.", id);
                return false;
            }

            _db.Playlists.Remove(pl); // PlaylistTracks се трият каскадно (FK ON DELETE CASCADE)
            await _db.SaveChangesAsync(ct);

            _logger.LogInformation("DeleteAsync: playlist {Id} deleted.", id);
            return true;
        }
    }
}