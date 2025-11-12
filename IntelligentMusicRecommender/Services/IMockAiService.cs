using IntelligentMusicRecommender.Models;

namespace IntelligentMusicRecommender.Services
{
    public record AiRecommendationRequest(string? Mood, string? Genre, int Limit = 15);

    public interface IMockAiService
    {
        Task<Dictionary<string, double>> BuildSeedAsync(AiRecommendationRequest request, CancellationToken ct = default);
        Task<IReadOnlyList<Track>> RankAsync(IEnumerable<Track> candidates, AiRecommendationRequest request, CancellationToken ct = default);
    }
}