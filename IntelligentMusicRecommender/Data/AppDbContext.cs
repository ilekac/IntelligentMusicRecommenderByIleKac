using Microsoft.EntityFrameworkCore;
using IntelligentMusicRecommender.Models;

namespace IntelligentMusicRecommender.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Track> Tracks => Set<Track>();
        public DbSet<Playlist> Playlists => Set<Playlist>();
        public DbSet<PlaylistTrack> PlaylistTracks => Set<PlaylistTrack>();
        public DbSet<UserPreference> UserPreferences => Set<UserPreference>();


        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<Track>().HasIndex(t => new { t.ExternalId, t.Source }).IsUnique();

            b.Entity<PlaylistTrack>()
                .HasOne(pt => pt.Playlist)
                .WithMany(p => p.Tracks)
                .HasForeignKey(pt => pt.PlaylistId);

            b.Entity<PlaylistTrack>()
                .HasOne(pt => pt.Track)
                .WithMany()
                .HasForeignKey(pt => pt.TrackId);
        }

    }
}