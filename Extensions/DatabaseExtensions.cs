using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using GamesSharp.Data;
using Microsoft.Data.Sqlite;
using System.Reflection;

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

                    logger.LogInformation("Начинается применение миграций базы данных");
                    await dbContext.Database.MigrateAsync();
                    logger.LogInformation("Миграции успешно применены");

                    if (providerName.Contains("Sqlite", StringComparison.OrdinalIgnoreCase))
                    {
                        // Legacy compatibility for SQLite databases previously created without migrations.
                        await EnsureSqliteSchemaCompatibilityAsync(dbContext, logger);
                    }
                }
                catch (SqliteException ex) when (ex.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase))
                {
                    logger.LogWarning(ex, "SQLite база создана без истории миграций. Выполняется fallback-инициализация");

                    await dbContext.Database.EnsureCreatedAsync();
                    await EnsureSqliteSchemaCompatibilityAsync(dbContext, logger);
                    await EnsureSqliteMigrationBaselineAsync(dbContext, logger);

                    logger.LogInformation("SQLite fallback-инициализация успешно завершена");
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

        private static async Task EnsureSqliteSchemaCompatibilityAsync(ApplicationDbContext dbContext, ILogger logger)
        {
            // For databases created by EnsureCreated in previous versions, add new schema parts incrementally.
            await dbContext.Database.ExecuteSqlRawAsync(@"
                CREATE TABLE IF NOT EXISTS ""VenueEquipments"" (
                    ""Id"" INTEGER NOT NULL CONSTRAINT ""PK_VenueEquipments"" PRIMARY KEY AUTOINCREMENT,
                    ""VenueId"" INTEGER NOT NULL,
                    ""EquipmentId"" INTEGER NOT NULL,
                    ""Quantity"" INTEGER NOT NULL,
                    CONSTRAINT ""FK_VenueEquipments_Venues_VenueId"" FOREIGN KEY (""VenueId"") REFERENCES ""Venues"" (""Id"") ON DELETE CASCADE,
                    CONSTRAINT ""FK_VenueEquipments_Equipments_EquipmentId"" FOREIGN KEY (""EquipmentId"") REFERENCES ""Equipments"" (""Id"") ON DELETE RESTRICT
                );");

            await dbContext.Database.ExecuteSqlRawAsync(@"
                CREATE UNIQUE INDEX IF NOT EXISTS ""IX_VenueEquipments_VenueId_EquipmentId""
                ON ""VenueEquipments"" (""VenueId"", ""EquipmentId"");");

            await EnsureSqliteColumnExistsAsync(dbContext, "Venues", "Latitude", "REAL");
            await EnsureSqliteColumnExistsAsync(dbContext, "Venues", "Longitude", "REAL");

            await dbContext.Database.ExecuteSqlRawAsync(@"
                INSERT OR IGNORE INTO ""VenueEquipments"" (""Id"", ""VenueId"", ""EquipmentId"", ""Quantity"") VALUES
                    (1, 1, 1, 40),
                    (2, 1, 2, 4),
                    (3, 1, 3, 2),
                    (4, 1, 4, 20),
                    (5, 2, 1, 25),
                    (6, 2, 2, 2),
                    (7, 2, 4, 10),
                    (8, 3, 1, 60),
                    (9, 3, 2, 8),
                    (10, 3, 3, 4),
                    (11, 3, 4, 35);");

            await dbContext.Database.ExecuteSqlRawAsync(@"
                UPDATE ""Venues"" SET ""Latitude"" = 55.7496, ""Longitude"" = 37.5927 WHERE ""Id"" = 1 AND ""Latitude"" IS NULL;");
            await dbContext.Database.ExecuteSqlRawAsync(@"
                UPDATE ""Venues"" SET ""Latitude"" = 55.7109, ""Longitude"" = 37.5865 WHERE ""Id"" = 2 AND ""Latitude"" IS NULL;");
            await dbContext.Database.ExecuteSqlRawAsync(@"
                UPDATE ""Venues"" SET ""Latitude"" = 55.7811, ""Longitude"" = 37.6347 WHERE ""Id"" = 3 AND ""Latitude"" IS NULL;");

            logger.LogInformation("SQLite schema compatibility checks applied");
        }

        private static async Task EnsureSqliteColumnExistsAsync(ApplicationDbContext dbContext, string tableName, string columnName, string columnType)
        {
            var connection = (SqliteConnection)dbContext.Database.GetDbConnection();
            var shouldClose = false;

            if (connection.State != System.Data.ConnectionState.Open)
            {
                await connection.OpenAsync();
                shouldClose = true;
            }

            try
            {
                await using var checkCommand = connection.CreateCommand();
                checkCommand.CommandText = $"PRAGMA table_info(\"{tableName}\");";

                var exists = false;
                await using (var reader = await checkCommand.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        if (string.Equals(reader[1]?.ToString(), columnName, StringComparison.OrdinalIgnoreCase))
                        {
                            exists = true;
                            break;
                        }
                    }
                }

                if (!exists)
                {
                    var alterSql = "ALTER TABLE \"" + tableName + "\" ADD COLUMN \"" + columnName + "\" " + columnType + " NULL;";
                    await dbContext.Database.ExecuteSqlRawAsync(alterSql);
                }
            }
            finally
            {
                if (shouldClose)
                {
                    await connection.CloseAsync();
                }
            }
        }

        private static async Task EnsureSqliteMigrationBaselineAsync(ApplicationDbContext dbContext, ILogger logger)
        {
            await dbContext.Database.ExecuteSqlRawAsync(@"
                CREATE TABLE IF NOT EXISTS ""__EFMigrationsHistory"" (
                    ""MigrationId"" TEXT NOT NULL CONSTRAINT ""PK___EFMigrationsHistory"" PRIMARY KEY,
                    ""ProductVersion"" TEXT NOT NULL
                );");

            var productVersion =
                typeof(DbContext).Assembly
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    ?.InformationalVersion
                    ?.Split('+')[0]
                ?? "10.0.0";

            foreach (var migrationId in dbContext.Database.GetMigrations())
            {
                await dbContext.Database.ExecuteSqlInterpolatedAsync(
                    $"INSERT OR IGNORE INTO \"__EFMigrationsHistory\" (\"MigrationId\", \"ProductVersion\") VALUES ({migrationId}, {productVersion});");
            }

            logger.LogInformation("SQLite baseline истории миграций синхронизирован");
        }
    }
}
