using IntelligentMusicRecommender.Data;
using IntelligentMusicRecommender.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IntelligentMusicRecommender.Controllers
{
    public class PreferencesController : Controller
    {
        private readonly AppDbContext _db;
        public PreferencesController(AppDbContext db) => _db = db;

        // GET /Preferences/Edit
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var pref = await _db.UserPreferences.OrderBy(x => x.Id).FirstOrDefaultAsync()
                       ?? new UserPreference();
            return View(pref);
        }

        // POST /Preferences/Edit
        [HttpPost]
        public async Task<IActionResult> Edit(UserPreference model)
        {
            var existing = await _db.UserPreferences.OrderBy(x => x.Id).FirstOrDefaultAsync();
            if (existing is null)
            {
                _db.UserPreferences.Add(model);
            }
            else
            {
                existing.Username = model.Username;
                existing.FavoriteGenre = model.FavoriteGenre;
                existing.MoodKeywords = model.MoodKeywords;
            }
            await _db.SaveChangesAsync();

            TempData["Success"] = "Preferences saved.";
            return RedirectToAction(nameof(Edit));
        }
    }
}