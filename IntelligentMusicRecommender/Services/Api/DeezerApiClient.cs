using System.Net.Http.Json;
using IntelligentMusicRecommender.Models;

namespace IntelligentMusicRecommender.Services.Api
{
    public class DeezerApiClient : IMusicApiClient
    {
        private readonly HttpClient _http;

        public DeezerApiClient(HttpClient http)
        {
            _http = http;
            _http.BaseAddress = new Uri("https://api.deezer.com/");
        }

        public async Task<IReadOnlyList<Track>> SearchTracksAsync(IEnumerable<string> keywords, int limit = 30, CancellationToken ct = default)
        {
            // комбинираме ключови думи в заявката
            var q = string.Join(" ", keywords.Where(k => !string.IsNullOrWhiteSpace(k)).Select(Uri.EscapeDataString));
            if (string.IsNullOrWhiteSpace(q)) q = "pop";

            var url = $"search?q={q}&limit={limit}";
            var res = await _http.GetFromJsonAsync<DeezerSearchResponse>(url, ct) ?? new DeezerSearchResponse();

            // преобразуваме Deezer JSON в нашия модел Track
            return res.Data.Select(d => new Track
            {
                ExternalId = d.id.ToString(),
                Title = d.title ?? "",
                Artist = d.artist?.name ?? "",
                Album = d.album?.title,
                PreviewUrl = d.preview,
                ArtworkUrl = d.album?.cover_medium ?? d.album?.cover,
                Source = "Deezer"
            }).ToList();
        }

        // вътрешни DTO класове за JSON отговора
        private class DeezerSearchResponse { public List<DeezerTrack> Data { get; set; } = new(); }
        private class DeezerTrack
        {
            public long id { get; set; }
            public string? title { get; set; }
            public string? preview { get; set; }
            public DeezerArtist? artist { get; set; }
            public DeezerAlbum? album { get; set; }
        }
        private class DeezerArtist { public string? name { get; set; } }
        private class DeezerAlbum
        {
            public string? title { get; set; }
            public string? cover { get; set; }
            public string? cover_medium { get; set; }
        }
    }
}