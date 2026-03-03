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

    public class ReferenceDataService : IReferenceDataService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly ILogger<ReferenceDataService> _logger;

        private const string CATEGORIES_CACHE_KEY = "game_categories";
        private const string PUBLISHERS_CACHE_KEY = "publishers";
        private const string EQUIPMENTS_CACHE_KEY = "equipments";
        private static readonly TimeSpan CACHE_DURATION = TimeSpan.FromHours(1);

        public ReferenceDataService(
            ApplicationDbContext context,
            IMemoryCache cache,
            ILogger<ReferenceDataService> logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
        }

        /// <summary>
        /// Получает список категорий игр с кешированием
        /// </summary>
        public async Task<List<GameCategory>> GetGameCategoriesAsync()
        {
            if (_cache.TryGetValue(CATEGORIES_CACHE_KEY, out List<GameCategory>? cachedCategories))
            {
                _logger.LogDebug("Категории получены из кеша");
                return cachedCategories ?? new List<GameCategory>();
            }

            _logger.LogDebug("Загрузка категорий из базы данных");
            var categories = await _context.GameCategories.AsNoTracking().ToListAsync();
            
            _cache.Set(CATEGORIES_CACHE_KEY, categories, new MemoryCacheEntryOptions().SetAbsoluteExpiration(CACHE_DURATION));
            return categories;
        }

        /// <summary>
        /// Получает список издателей с кешированием
        /// </summary>
        public async Task<List<Publisher>> GetPublishersAsync()
        {
            if (_cache.TryGetValue(PUBLISHERS_CACHE_KEY, out List<Publisher>? cachedPublishers))
            {
                _logger.LogDebug("Издатели получены из кеша");
                return cachedPublishers ?? new List<Publisher>();
            }

            _logger.LogDebug("Загрузка издателей из базы данных");
            var publishers = await _context.Publishers.AsNoTracking().ToListAsync();
            
            _cache.Set(PUBLISHERS_CACHE_KEY, publishers, new MemoryCacheEntryOptions().SetAbsoluteExpiration(CACHE_DURATION));
            return publishers;
        }

        /// <summary>
        /// Получает список оборудования с кешированием
        /// </summary>
        public async Task<List<Equipment>> GetEquipmentsAsync()
        {
            if (_cache.TryGetValue(EQUIPMENTS_CACHE_KEY, out List<Equipment>? cachedEquipments))
            {
                _logger.LogDebug("Оборудование получено из кеша");
                return cachedEquipments ?? new List<Equipment>();
            }

            _logger.LogDebug("Загрузка оборудования из базы данных");
            var equipments = await _context.Equipments.AsNoTracking().ToListAsync();
            
            _cache.Set(EQUIPMENTS_CACHE_KEY, equipments, new MemoryCacheEntryOptions().SetAbsoluteExpiration(CACHE_DURATION));
            return equipments;
        }

        /// <summary>
        /// Инвалидирует весь кеш справочных данных (используется при изменении данных)
        /// </summary>
        public Task InvalidateCacheAsync()
        {
            _logger.LogInformation("Инвалидация кеша справочных данных");
            _cache.Remove(CATEGORIES_CACHE_KEY);
            _cache.Remove(PUBLISHERS_CACHE_KEY);
            _cache.Remove(EQUIPMENTS_CACHE_KEY);
            return Task.CompletedTask;
        }
    }
}
