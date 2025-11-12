using IntelligentMusicRecommender.Models;

namespace IntelligentMusicRecommender.Services
{
    public class MockAiService : IMockAiService
    {
        private static readonly Dictionary<string, string[]> MoodMap = new(StringComparer.OrdinalIgnoreCase)
        {
            ["happy"] = new[] { "dance", "pop", "feel good" },
            ["sad"] = new[] { "acoustic", "piano", "ballad" },
            ["focus"] = new[] { "lofi", "instrumental", "chill" },
            ["gym"] = new[] { "edm", "trap", "workout" },
            ["party"] = new[] { "club", "dance", "electro" }
        };

        public Task<Dictionary<string, double>> BuildSeedAsync(AiRecommendationRequest req, CancellationToken ct = default)
        {
            var dict = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

            if (!string.IsNullOrWhiteSpace(req.Genre))
                dict[req.Genre!.Trim()] = 1.0;

            if (!string.IsNullOrWhiteSpace(req.Mood) && MoodMap.TryGetValue(req.Mood!.Trim(), out var kws))
                foreach (var k in kws) dict[k] = dict.ContainsKey(k) ? dict[k] + .2 : .8;

            if (dict.Count == 0)
            {
                foreach (var k in new[] { "pop", "top hits", "trending" })
                    dict[k] = .5;
            }

            return Task.FromResult(dict);
        }

        public Task<IReadOnlyList<Track>> RankAsync(IEnumerable<Track> candidates, AiRecommendationRequest req, CancellationToken ct = default)
        {
            var words = ((req.Mood ?? "") + " " + (req.Genre ?? ""))
                        .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .Select(w => w.ToLowerInvariant()).ToHashSet();

            var result = candidates
                .GroupBy(t => (t.Artist.ToLowerInvariant(), t.Title.ToLowerInvariant()))
                .Select(g => g.First())
                .Select(t => (t, score:
                    (words.Any(w => t.Title.ToLower().Contains(w)) ? 1.0 : 0) +
                    (words.Any(w => t.Artist.ToLower().Contains(w)) ? 0.5 : 0)))
                .OrderByDescending(x => x.score)
                .ThenBy(x => x.t.Title)
                .Select(x => x.t)
                .ToList();

            return Task.FromResult<IReadOnlyList<Track>>(result);
        }
    }
}