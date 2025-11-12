using IntelligentMusicRecommender.Services;
using Microsoft.AspNetCore.Mvc;

namespace IntelligentMusicRecommender.Controllers
{
    public class PlaylistsController : Controller
    {
        private readonly IPlaylistService _svc;
        public PlaylistsController(IPlaylistService svc) => _svc = svc;

        // /Playlists
        public async Task<IActionResult> Index() => View(await _svc.GetAllAsync());

        // /Playlists/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var pl = await _svc.GetPlaylistAsync(id);
            return pl is null ? NotFound() : View(pl);
        }

        // POST: /Playlists/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var ok = await _svc.DeleteAsync(id, ct);
            TempData[ok ? "Success" : "Error"] = ok
                ? "Playlist deleted successfully."
                : "Playlist not found.";

            return RedirectToAction(nameof(Index));
        }
    }
}