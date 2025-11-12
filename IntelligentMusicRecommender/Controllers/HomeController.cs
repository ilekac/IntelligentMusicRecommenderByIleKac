using System.Diagnostics;
using IntelligentMusicRecommender.Data;
using IntelligentMusicRecommender.Models;
using IntelligentMusicRecommender.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IntelligentMusicRecommender.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPlaylistService _playlist;
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _db;

        public HomeController(IPlaylistService playlist, ILogger<HomeController> logger, AppDbContext db)
        {
            _playlist = playlist;
            _logger = logger;
            _db = db;
        }

        [HttpGet]
        public IActionResult Index() => View();

        // POST: Recommend playlist
        [HttpPost]
        public async Task<IActionResult> Recommend(string? mood, string? genre, int limit = 12, CancellationToken ct = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(mood) && string.IsNullOrWhiteSpace(genre))
                    genre = "pop";
                if (limit is < 5 or > 50) limit = 12;

                var pl = await _playlist.CreateRecommendedAsync(mood, genre, limit, ct);

                _logger.LogInformation("Playlist {PlaylistId} created via Home.Recommend.", pl.Id);
                TempData["Success"] = "Playlist created successfully!";
                return RedirectToAction("Details", "Playlists", new { id = pl.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Home.Recommend failed. mood={Mood}, genre={Genre}, limit={Limit}", mood, genre, limit);
                TempData["Error"] = "Oops — couldn’t generate recommendations right now. Try again later.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Load saved preferences into form (auto-fill)
        [HttpGet]
        public async Task<IActionResult> UseDefaults(CancellationToken ct)
        {
            var pref = await _db.UserPreferences.OrderBy(x => x.Id).FirstOrDefaultAsync(ct);
            if (pref is null)
            {
                TempData["Error"] = "No saved preferences yet.";
                return RedirectToAction(nameof(Index));
            }

            // Извеждаме в логовете и показваме зелен банер
            _logger.LogInformation("Loaded defaults: genre={Genre}, mood={Mood}", pref.FavoriteGenre, pref.MoodKeywords);
            TempData["Success"] = "Loaded your saved preferences.";

            // Пренасочваме към Index с mood/genre в URL-а
            return RedirectToAction(nameof(Index), new
            {
                mood = pref.MoodKeywords,
                genre = pref.FavoriteGenre
            });
        }

        [HttpGet]
        public IActionResult Privacy() => View();

        // 500 – centralized error page
        [HttpGet]
        public IActionResult Error()
        {
            Response.StatusCode = 500;
            var vm = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };
            return View(vm);
        }

        // 404/403/... – friendly error pages
        [HttpGet]
        public IActionResult StatusCode(int code)
        {
            Response.StatusCode = code;
            return View("StatusCode", code);
        }
    }
}