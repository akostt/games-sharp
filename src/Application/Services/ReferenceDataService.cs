using GamesSharp.Data;
using GamesSharp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace GamesSharp.Services
{
    /// <summary>
    /// Сервис для управления справочными данными с кешированием
    /// </summary>
    public interface IReferenceDataService
    {
        Task<List<GameCategory>> GetGameCategoriesAsync();
        Task<List<Publisher>> GetPublishersAsync();
        Task<List<Equipment>> GetEquipmentsAsync();
        Task InvalidateCacheAsync();
    }

    public class ReferenceDataService(
        ApplicationDbContext context,
        IMemoryCache cache,
        ILogger<ReferenceDataService> logger)
        : IReferenceDataService
    {
        private const string CATEGORIES_CACHE_KEY = "game_categories";
        private const string PUBLISHERS_CACHE_KEY = "publishers";
        private const string EQUIPMENTS_CACHE_KEY = "equipments";
        private static readonly TimeSpan CACHE_DURATION = TimeSpan.FromHours(1);

        /// <summary>
        /// Получает список категорий игр с кешированием
        /// </summary>
        public async Task<List<GameCategory>> GetGameCategoriesAsync()
        {
            if (cache.TryGetValue(CATEGORIES_CACHE_KEY, out List<GameCategory>? cachedCategories))
            {
                logger.LogDebug("Категории получены из кеша");
                return cachedCategories ?? new List<GameCategory>();
            }

            logger.LogDebug("Загрузка категорий из базы данных");
            var categories = await context.GameCategories.AsNoTracking().ToListAsync();
            
            cache.Set(CATEGORIES_CACHE_KEY, categories, new MemoryCacheEntryOptions().SetAbsoluteExpiration(CACHE_DURATION));
            return categories;
        }

        /// <summary>
        /// Получает список издателей с кешированием
        /// </summary>
        public async Task<List<Publisher>> GetPublishersAsync()
        {
            if (cache.TryGetValue(PUBLISHERS_CACHE_KEY, out List<Publisher>? cachedPublishers))
            {
                logger.LogDebug("Издатели получены из кеша");
                return cachedPublishers ?? new List<Publisher>();
            }

            logger.LogDebug("Загрузка издателей из базы данных");
            var publishers = await context.Publishers
                .Include(p => p.Country)
                .AsNoTracking()
                .ToListAsync();
            
            cache.Set(PUBLISHERS_CACHE_KEY, publishers, new MemoryCacheEntryOptions().SetAbsoluteExpiration(CACHE_DURATION));
            return publishers;
        }

        /// <summary>
        /// Получает список оборудования с кешированием
        /// </summary>
        public async Task<List<Equipment>> GetEquipmentsAsync()
        {
            if (cache.TryGetValue(EQUIPMENTS_CACHE_KEY, out List<Equipment>? cachedEquipments))
            {
                logger.LogDebug("Оборудование получено из кеша");
                return cachedEquipments ?? new List<Equipment>();
            }

            logger.LogDebug("Загрузка оборудования из базы данных");
            var equipments = await context.Equipments
                .Include(e => e.EquipmentType)
                .AsNoTracking()
                .ToListAsync();
            
            cache.Set(EQUIPMENTS_CACHE_KEY, equipments, new MemoryCacheEntryOptions().SetAbsoluteExpiration(CACHE_DURATION));
            return equipments;
        }

        /// <summary>
        /// Инвалидирует весь кеш справочных данных (используется при изменении данных)
        /// </summary>
        public Task InvalidateCacheAsync()
        {
            logger.LogInformation("Инвалидация кеша справочных данных");
            cache.Remove(CATEGORIES_CACHE_KEY);
            cache.Remove(PUBLISHERS_CACHE_KEY);
            cache.Remove(EQUIPMENTS_CACHE_KEY);
            return Task.CompletedTask;
        }
    }
}
