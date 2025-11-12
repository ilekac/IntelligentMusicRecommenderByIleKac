namespace IntelligentMusicRecommender.Models
{
    public class Playlist
    {
        public int Id { get; set; }
        public string Name { get; set; } = "My AI Playlist";
        public string? Criteria { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<PlaylistTrack> Tracks { get; set; } = new();
    }

    public class PlaylistTrack
    {
        public int Id { get; set; }
        public int PlaylistId { get; set; }
        public Playlist Playlist { get; set; } = null!;
        public int TrackId { get; set; }
        public Track Track { get; set; } = null!;
        public int Order { get; set; }
    }
}
