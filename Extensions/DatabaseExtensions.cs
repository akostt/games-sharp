using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using GamesSharp.Data;

namespace GamesSharp.Extensions
{
    /// <summary>
    /// Расширения для инициализации и миграции базы данных
    /// </summary>
    public static class DatabaseExtensions
    {
        /// <summary>
        /// Инициализирует БД с применением миграций и обработкой ошибок
        /// </summary>
        public static async Task InitializeDatabaseAsync(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

                try
                {
                    var providerName = dbContext.Database.ProviderName ?? string.Empty;
                    if (providerName.Contains("Sqlite", StringComparison.OrdinalIgnoreCase))
                    {
                        logger.LogInformation("Для SQLite выполняется EnsureCreated (без миграций)");
                        await dbContext.Database.EnsureCreatedAsync();
                        logger.LogInformation("База SQLite успешно инициализирована");
                    }
                    else
                    {
                        logger.LogInformation("Начинается применение миграций базы данных");
                        await dbContext.Database.MigrateAsync();
                        logger.LogInformation("Миграции успешно применены");
                    }
                }
                catch (InvalidOperationException ex)
                {
                    logger.LogError(ex, "Ошибка конфигурации БД. Проверьте строку подключения и учетные данные");
                    throw;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Критическая ошибка при применении миграций");
                    throw;
                }
            }
        }

        /// <summary>
        /// Регистрирует DbContext с правильными параметрами подключения
        /// </summary>
        public static IServiceCollection AddApplicationDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            string provider = configuration["Database:Provider"]?.Trim().ToLowerInvariant() ?? "sqlite";

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                switch (provider)
                {
                    case "sqlserver":
                        var sqlServerConnection = configuration.GetConnectionString("SqlServerConnection")
                            ?? configuration.GetConnectionString("DefaultConnection")
                            ?? throw new InvalidOperationException("Не найдена строка подключения SqlServerConnection или DefaultConnection");

                        options.UseSqlServer(
                            sqlServerConnection,
                            sqlOptions => sqlOptions
                                .MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
                                .EnableRetryOnFailure(
                                    maxRetryCount: 3,
                                    maxRetryDelay: TimeSpan.FromSeconds(5),
                                    errorNumbersToAdd: null));
                        options.ConfigureWarnings(warnings =>
                            warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
                        break;

                    case "sqlite":
                        var sqliteConnection = configuration.GetConnectionString("SqliteConnection")
                            ?? "Data Source=games-sharp.db";

                        options.UseSqlite(
                            sqliteConnection,
                            sqliteOptions => sqliteOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
                        options.ConfigureWarnings(warnings =>
                            warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
                        break;

                    default:
                        throw new InvalidOperationException(
                            $"Неподдерживаемый провайдер БД: '{provider}'. Допустимые значения: sqlite, sqlserver");
                }
            }, ServiceLifetime.Scoped);

            return services;
        }
    }
}
