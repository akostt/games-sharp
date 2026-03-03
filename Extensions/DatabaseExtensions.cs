using Microsoft.EntityFrameworkCore;
using GamesSharp.Data;
using System.Runtime.InteropServices;

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
                    logger.LogInformation("Начинается применение миграций базы данных");
                    await dbContext.Database.MigrateAsync();
                    logger.LogInformation("Миграции успешно применены");
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
            string connectionString = GetConnectionString(configuration);
            
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    connectionString,
                    sqlOptions => sqlOptions
                        .MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
                        .EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(5), errorNumbersToAdd: null)
                ),
                ServiceLifetime.Scoped);

            return services;
        }

        /// <summary>
        /// Получает строку подключения в зависимости от ОС
        /// </summary>
        private static string GetConnectionString(IConfiguration configuration)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "Server=localhost;Database=Games;Integrated Security=true;TrustServerCertificate=True;";
            }

            return configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Строка подключения 'DefaultConnection' не найдена в конфигурации");
        }
    }
}
