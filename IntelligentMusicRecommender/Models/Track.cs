namespace IntelligentMusicRecommender.Models
{
    public class Track
    {
        public int Id { get; set; }
        public string ExternalId { get; set; } = "";
        public string Title { get; set; } = "";
        public string Artist { get; set; } = "";
        public string? Album { get; set; }
        public string? PreviewUrl { get; set; }
        public string? ArtworkUrl { get; set; }
        public string Source { get; set; } = "Deezer";
    }
}