IntelligentMusicRecommenderByIleKac
Интелигентен уеб препоръчител на музика (ASP.NET Core MVC)

Описание

IntelligentMusicRecommenderByIleKac е уеб приложение, което предлага персонализирани музикални препоръки според въведено настроение или жанр.
Логиката комбинира симулиран изкуствен интелект (Mock AI), външен Deezer API клиент и локална SQLite база за съхранение на плейлисти и предпочитания.

Функционалности

Въвеждане на mood и genre

Генериране на плейлист чрез Mock AI логика

Извличане на реални песни чрез Deezer API

Съхранение на песни и плейлисти в SQLite (music.db)

Преглед на всички плейлисти

Преглед на детайли на конкретен плейлист

Изтриване на избран плейлист

Запазване на потребителски предпочитания (Preferences)

Автопопълване на формата чрез Use Defaults

Глобално логване и error handling

Модерен UI с тема purple / black / white



Използвани технологии

ASP.NET Core MVC 8

C#

Entity Framework Core (SQLite)

Razor Views

HttpClient (Deezer API)

Dependency Injection

Bootstrap 5 + Custom CSS

ILogger Logging

Async/Await асинхронност

Структура на проекта:
Controllers/           // Контролери – Home, Playlists, Preferences
Models/                // Модели – Track, Playlist, PlaylistTrack, UserPreference
Views/                 // Razor изгледи – Home, Playlists, Preferences
Services/              // Логика – PlaylistService, MockAiService
Services/Api/          // DeezerApiClient (външен API клиент)
Data/                  // AppDbContext + миграции
wwwroot/               // CSS, JS, Bootstrap
music.db               // SQLite база
Dockerfile             // Docker конфигурация
Program.cs             // Стартов файл на приложението


Инсталация и стартиране (локално):
git clone https://github.com/ilekac/IntelligentMusicRecommenderByIleKac.git
cd IntelligentMusicRecommenderByIleKac

Миграции и база данни:
Add-Migration Init
Update-Database


Стартиране:
dotnet run

След това отвори в браузър:
http://localhost:5000
или 
https://localhost:7000
