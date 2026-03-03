using GamesSharp.Data;
using GamesSharp.Extensions;

var builder = WebApplication.CreateBuilder(args);

// ════════════════════════════════════════════════════════════════════════════════
// Регистрация сервисов
// ════════════════════════════════════════════════════════════════════════════════

// Основные сервисы ASP.NET Core
builder.Services.AddControllersWithViews();

// База данных с автоматической миграцией и обработкой ошибок
builder.Services.AddApplicationDbContext(builder.Configuration);

// Приложение-специфичные сервисы (кеш, CORS, и т.д.)
builder.Services.AddApplicationServices();

var app = builder.Build();

// ════════════════════════════════════════════════════════════════════════════════
// Инициализация базы данных
// ════════════════════════════════════════════════════════════════════════════════

await app.InitializeDatabaseAsync();

// ════════════════════════════════════════════════════════════════════════════════
// Конфигурация middleware и маршрутизация
// ════════════════════════════════════════════════════════════════════════════════

app.UseApplicationMiddleware();

app.Run();
