using Microsoft.EntityFrameworkCore;
using IntelligentMusicRecommender.Data;
using IntelligentMusicRecommender.Services;
using IntelligentMusicRecommender.Services.Api;

var builder = WebApplication.CreateBuilder(args);

// =============================================
//  LOGGING CONFIGURATION
// =============================================
// Изчистваме стандартните лог провайдъри и активираме изход към
// Visual Studio → Output → Debug и конзолата.
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// =============================================
//  SERVICES CONFIGURATION
// =============================================

// 1) MVC контролери и изгледи (Razor Views)
builder.Services.AddControllersWithViews();

// 2) Entity Framework Core – връзка с базата
//    (чете connection string "Default" от appsettings.json)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=music.db"));

// 3) HttpClient – Deezer API
// Добавяме timeout (10 секунди) и user-agent за идентификация
builder.Services.AddHttpClient<IMusicApiClient, DeezerApiClient>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(10);
    client.DefaultRequestHeaders.UserAgent.ParseAdd("AI-Music-RecommenderByIleKac/1.0");
});

// 4) Mock AI и Playlist услуги
builder.Services.AddScoped<IMockAiService, MockAiService>();
builder.Services.AddScoped<IPlaylistService, PlaylistService>();

// =============================================
//  APP PIPELINE CONFIGURATION
// =============================================

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    // Глобален error handler за production режим
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

// 404/403/… приятни страници (UseStatusCodePagesWithReExecute)
app.UseStatusCodePagesWithReExecute("/Home/StatusCode", "?code={0}");

// Дефинираме стандартния маршрут: /Home/Index/{id?}
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Стартира приложението
app.Run();