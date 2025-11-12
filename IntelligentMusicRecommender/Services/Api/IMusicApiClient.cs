using IntelligentMusicRecommender.Models;

namespace IntelligentMusicRecommender.Services.Api
{
    public interface IMusicApiClient
    {
        Task<IReadOnlyList<Track>> SearchTracksAsync(IEnumerable<string> keywords, int limit = 30, CancellationToken ct = default);
    }
}