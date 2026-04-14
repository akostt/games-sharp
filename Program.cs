using GamesSharp.Data;
using GamesSharp.Extensions;

var builder = WebApplication.CreateBuilder(args);

// ════════════════════════════════════════════════════════════════════════════════
// Регистрация сервисов
// ════════════════════════════════════════════════════════════════════════════════

// Основные сервисы ASP.NET Core
builder.Services.AddControllersWithViews(options =>
{
	var provider = options.ModelBindingMessageProvider;
	provider.SetValueMustNotBeNullAccessor(_ => "Поле обязательно для заполнения.");
	provider.SetValueIsInvalidAccessor(_ => "Указано некорректное значение.");
	provider.SetValueMustBeANumberAccessor(_ => "Поле должно быть числом.");
	provider.SetAttemptedValueIsInvalidAccessor((_, _) => "Указано недопустимое значение.");
	provider.SetUnknownValueIsInvalidAccessor(_ => "Указано недопустимое значение.");
	provider.SetNonPropertyValueMustBeANumberAccessor(() => "Значение должно быть числом.");
	provider.SetNonPropertyAttemptedValueIsInvalidAccessor(_ => "Указано недопустимое значение.");
	provider.SetNonPropertyUnknownValueIsInvalidAccessor(() => "Указано недопустимое значение.");
	provider.SetMissingBindRequiredValueAccessor(_ => "Поле обязательно для заполнения.");
	provider.SetMissingKeyOrValueAccessor(() => "Требуется значение.");
	provider.SetMissingRequestBodyRequiredValueAccessor(() => "Тело запроса обязательно.");
});

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
