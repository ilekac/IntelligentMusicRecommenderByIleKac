namespace IntelligentMusicRecommender.Models
{
    public class UserPreference
    {
        public int Id { get; set; }

        // Показваме го във формата чисто информативно
        public string? Username { get; set; }

        // Ще го ползваме за жанр по подразбиране
        public string? FavoriteGenre { get; set; }

        // Кома-разделени ключови думи: happy, focus, gym ...
        public string? MoodKeywords { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}