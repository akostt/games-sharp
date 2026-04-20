using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GamesSharp.Data;
using GamesSharp.Models;
using GamesSharp.Helpers;
using GamesSharp.Services;

namespace GamesSharp.Controllers
{
    public class GamesController : BaseController
    {
        private readonly IReferenceDataService _referenceDataService;
        private readonly IExcelExportService _excelExportService;

        public GamesController(
            ApplicationDbContext context, 
            ILogger<GamesController> logger,
            IReferenceDataService referenceDataService,
            IExcelExportService excelExportService)
            : base(context, logger)
        {
            _referenceDataService = referenceDataService ?? throw new ArgumentNullException(nameof(referenceDataService));
            _excelExportService = excelExportService ?? throw new ArgumentNullException(nameof(excelExportService));
        }

        // GET: Games
        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            try
            {
                const int pageSize = Constants.Validation.DefaultPageSize;
                
                var query = Context.Games
                    .Include(g => g.GameCategoryAssignments)
                        .ThenInclude(gca => gca.GameCategory)
                    .Include(g => g.Publisher)
                    .AsNoTracking()
                    .OrderByDescending(g => g.Id);
                
                var paginatedGames = await PaginatedList<Game>.CreateAsync(query, pageNumber, pageSize);
                
                Logger.LogInformation("Загружена страница {PageNumber} итоговых игр {TotalCount}", pageNumber, paginatedGames.TotalCount);
                return View(paginatedGames);
            }
            catch (Exception ex)
            {
                return HandleException(ex, nameof(Index));
            }
        }

        // GET: Games/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!IsValidId(id))
                return NotFoundWithLogging("Игра", id);

            try
            {
                var game = await Context.Games
                    .Include(g => g.GameCategoryAssignments)
                        .ThenInclude(gca => gca.GameCategory)
                    .Include(g => g.Publisher)
                    .Include(g => g.GameEquipments)
                        .ThenInclude(ge => ge.Equipment)
                            .ThenInclude(e => e.EquipmentType)
                    .Include(g => g.GameSessions)
                        .ThenInclude(gs => gs.Venue)
                    .Include(g => g.GameSessions)
                        .ThenInclude(gs => gs.SessionStatus)
                    .Include(g => g.GameSessions)
                        .ThenInclude(gs => gs.SessionPlayers)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.Id == id);
                
                if (game == null)
                    return NotFoundWithLogging("Игра", id);

                Logger.LogInformation("Просмотр деталей игры: {GameName} (ID: {GameId})", game.Name, game.Id);
                return View(game);
            }
            catch (Exception ex)
            {
                return HandleException(ex, nameof(Details));
            }
        }

        // GET: Games/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                await PopulateViewBagAsync();
                return View();
            }
            catch (Exception ex)
            {
                return HandleException(ex, nameof(Create));
            }
        }

        // POST: Games/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,Name,Description,MinPlayers,MaxPlayers,AverageDuration,Complexity,MinAge,YearPublished,PublisherId")] Game game, 
            List<int>? selectedCategoryIds, 
            List<int>? selectedEquipment, 
            Dictionary<int, int>? equipmentQuantities)
        {
            ValidateGameData(game);
            ValidateEquipmentQuantities(selectedEquipment, equipmentQuantities);

            if (ModelState.IsValid)
            {
                try
                {
                    await using var transaction = await Context.Database.BeginTransactionAsync();

                    Context.Add(game);
                    await Context.SaveChangesAsync();

                    // Добавление категорий
                    await AddGameCategoriesAsync(game.Id, selectedCategoryIds);

                    // Добавление оборудования
                    await AddGameEquipmentsAsync(game.Id, selectedEquipment, equipmentQuantities);

                    await Context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    
                    Logger.LogInformation("Создана новая игра: {GameName} (ID: {GameId})", game.Name, game.Id);
                    SetSuccessMessage(Constants.SuccessMessages.RecordCreated);
                    
                    await _referenceDataService.InvalidateCacheAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    Logger.LogError(ex, "Ошибка БД при создании игры");
                    SetErrorMessage(Constants.ErrorMessages.DatabaseError);
                }
                catch (Exception ex)
                {
                    return HandleException(ex, nameof(Create));
                }
            }
            
            await PopulateViewBagAsync(selectedCategoryIds, game.PublisherId);
            return View(game);
        }

        // GET: Games/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsValidId(id))
                return NotFoundWithLogging("Игра", id);

            try
            {
                var game = await Context.Games
                    .Include(g => g.GameCategoryAssignments)
                    .Include(g => g.GameEquipments)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(g => g.Id == id);
                    
                if (game == null)
                    return NotFoundWithLogging("Игра", id);

                var selectedCategoryIds = game.GameCategoryAssignments
                    .Select(gca => gca.GameCategoryId)
                    .ToList();
                    
                await PopulateViewBagAsync(selectedCategoryIds, game.PublisherId, game.Id);
                return View(game);
            }
            catch (Exception ex)
            {
                return HandleException(ex, nameof(Edit));
            }
        }

        // POST: Games/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id, 
            [Bind("Id,Name,Description,MinPlayers,MaxPlayers,AverageDuration,Complexity,MinAge,YearPublished,PublisherId")] Game game, 
            List<int>? selectedCategoryIds, 
            List<int>? selectedEquipment, 
            Dictionary<int, int>? equipmentQuantities)
        {
            if (id != game.Id)
                return NotFoundWithLogging("Игра", id);

            ValidateGameData(game);
            ValidateEquipmentQuantities(selectedEquipment, equipmentQuantities);

            if (ModelState.IsValid)
            {
                try
                {
                    await using var transaction = await Context.Database.BeginTransactionAsync();

                    Context.Update(game);
                    await Context.SaveChangesAsync();

                    // Обновление категорий
                    await UpdateGameCategoriesAsync(game.Id, selectedCategoryIds);

                    // Обновление оборудования
                    await UpdateGameEquipmentsAsync(game.Id, selectedEquipment, equipmentQuantities);

                    await Context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    
                    Logger.LogInformation("Обновлена игра: {GameName} (ID: {GameId})", game.Name, game.Id);
                    SetSuccessMessage(Constants.SuccessMessages.RecordUpdated);
                    
                    await _referenceDataService.InvalidateCacheAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!await GameExistsAsync(game.Id))
                    {
                        return NotFoundWithLogging("Игра", game.Id);
                    }
                    
                    Logger.LogError(ex, "Ошибка конкурентности при обновлении игры ID: {GameId}", game.Id);
                    SetErrorMessage(Constants.ErrorMessages.ConcurrencyError);
                }
                catch (DbUpdateException ex)
                {
                    Logger.LogError(ex, "Ошибка БД при обновлении игры");
                    SetErrorMessage(Constants.ErrorMessages.DatabaseError);
                }
                catch (Exception ex)
                {
                    return HandleException(ex, nameof(Edit));
                }
            }
            
            await PopulateViewBagAsync(selectedCategoryIds, game.PublisherId, game.Id);
            return View(game);
        }

        // GET: Games/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsValidId(id))
                return NotFoundWithLogging("Игра", id);

            try
            {
                var game = await Context.Games
                    .Include(g => g.GameCategoryAssignments)
                        .ThenInclude(gca => gca.GameCategory)
                    .Include(g => g.Publisher)
                    .Include(g => g.GameSessions)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.Id == id);
                
                if (game == null)
                    return NotFoundWithLogging("Игра", id);

                return View(game);
            }
            catch (Exception ex)
            {
                return HandleException(ex, nameof(Delete));
            }
        }

        // GET: Games/ExportToExcel
        [HttpGet]
        public async Task<IActionResult> ExportToExcel(CancellationToken cancellationToken)
        {
            try
            {
                var games = await Context.Games
                    .Include(g => g.GameCategoryAssignments)
                        .ThenInclude(gca => gca.GameCategory)
                    .Include(g => g.Publisher)
                    .AsNoTracking()
                    .OrderBy(g => g.Name)
                    .ToListAsync(cancellationToken);

                var exportResult = await _excelExportService.ExportGamesAsync(games, cancellationToken);

                Logger.LogInformation("Сформирован экспорт игр в Excel. Записей: {Count}", games.Count);
                return PhysicalFile(exportResult.StoredFilePath, exportResult.ContentType, exportResult.DownloadFileName);
            }
            catch (Exception ex)
            {
                return HandleException(ex, nameof(ExportToExcel));
            }
        }

        // POST: Games/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var game = await Context.Games
                    .Include(g => g.GameSessions)
                    .FirstOrDefaultAsync(g => g.Id == id);
                    
                if (game == null)
                    return NotFoundWithLogging("Игра", id);

                // Проверка на связанные данные
                if (game.GameSessions.Any())
                {
                    Logger.LogWarning("Попытка удалить игру {GameName} (ID: {GameId}) которая имеет активные сессии", game.Name, game.Id);
                    SetErrorMessage("Невозможно удалить игру, которая имеет активные сессии.");
                    return RedirectToAction(nameof(Index));
                }

                Context.Games.Remove(game);
                await Context.SaveChangesAsync();
                
                Logger.LogInformation("Удалена игра: {GameName} (ID: {GameId})", game.Name, game.Id);
                SetSuccessMessage(Constants.SuccessMessages.RecordDeleted);
                
                await _referenceDataService.InvalidateCacheAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                Logger.LogError(ex, "Ошибка БД при удалении игры");
                SetErrorMessage(Constants.ErrorMessages.DeleteError);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return HandleException(ex, nameof(DeleteConfirmed));
            }
        }

        // ════════════════════════════════════════════════════════════════════════════════
        // Вспомогательные методы
        // ════════════════════════════════════════════════════════════════════════════════

        private async Task<bool> GameExistsAsync(int id)
        {
            return await Context.Games.AnyAsync(e => e.Id == id);
        }

        private async Task PopulateViewBagAsync(List<int>? selectedCategoryIds = null, int? publisherId = null, int? gameId = null)
        {
            var publisherList = await _referenceDataService.GetPublishersAsync();
            ViewBag.PublisherId = new SelectList(publisherList, "Id", "Name", publisherId);

            var categoryList = await _referenceDataService.GetGameCategoriesAsync();
            ViewBag.AllCategories = categoryList;
            ViewBag.SelectedCategoryIds = selectedCategoryIds ?? new List<int>();

            await PopulateEquipmentViewBagAsync(gameId);
        }

        private async Task PopulateEquipmentViewBagAsync(int? gameId = null)
        {
            var allEquipment = await _referenceDataService.GetEquipmentsAsync();
            var selectedEquipment = new List<int>();
            var equipmentQuantities = new Dictionary<int, int>();

            if (gameId.HasValue)
            {
                var gameEquipments = await Context.GameEquipments
                    .AsNoTracking()
                    .Where(ge => ge.GameId == gameId.Value)
                    .ToListAsync();
                    
                selectedEquipment = gameEquipments.Select(ge => ge.EquipmentId).ToList();
                equipmentQuantities = gameEquipments.ToDictionary(ge => ge.EquipmentId, ge => ge.RequiredQuantity);
            }

            ViewBag.AllEquipment = allEquipment;
            ViewBag.SelectedEquipment = selectedEquipment;
            ViewBag.EquipmentQuantities = equipmentQuantities;
        }

        private Task AddGameCategoriesAsync(int gameId, List<int>? selectedCategoryIds)
        {
            if (selectedCategoryIds == null || !selectedCategoryIds.Any())
            return Task.CompletedTask;

            var categoryAssignments = selectedCategoryIds
                .Distinct()
                .Select(categoryId => new GameCategoryAssignment
                {
                    GameId = gameId,
                    GameCategoryId = categoryId
                })
                .ToList();

            Context.GameCategoryAssignments.AddRange(categoryAssignments);
            return Task.CompletedTask;
        }

        private async Task UpdateGameCategoriesAsync(int gameId, List<int>? selectedCategoryIds)
        {
            var existingCategories = await Context.GameCategoryAssignments
                .Where(gca => gca.GameId == gameId)
                .ToListAsync();
                
            Context.GameCategoryAssignments.RemoveRange(existingCategories);
            await AddGameCategoriesAsync(gameId, selectedCategoryIds);
        }

        private Task AddGameEquipmentsAsync(int gameId, List<int>? selectedEquipment, Dictionary<int, int>? equipmentQuantities)
        {
            if (selectedEquipment == null || !selectedEquipment.Any())
            return Task.CompletedTask;

            var gameEquipments = selectedEquipment
                .Select(equipmentId => new GameEquipment
                {
                    GameId = gameId,
                    EquipmentId = equipmentId,
                    RequiredQuantity = equipmentQuantities?.ContainsKey(equipmentId) == true 
                        ? equipmentQuantities[equipmentId] 
                        : 1
                })
                .ToList();

            Context.GameEquipments.AddRange(gameEquipments);
            return Task.CompletedTask;
        }

        private async Task UpdateGameEquipmentsAsync(int gameId, List<int>? selectedEquipment, Dictionary<int, int>? equipmentQuantities)
        {
            var existingEquipment = await Context.GameEquipments
                .Where(ge => ge.GameId == gameId)
                .ToListAsync();
                
            Context.GameEquipments.RemoveRange(existingEquipment);
            await AddGameEquipmentsAsync(gameId, selectedEquipment, equipmentQuantities);
        }

        /// <summary>
        /// Удалит дополнительную валидацию данных игры
        /// </summary>
        private void ValidateGameData(Game game)
        {
            // Перепроверка обязательных полей на уровне контроллера
            if (string.IsNullOrWhiteSpace(game.Name))
                ModelState.AddModelError(nameof(game.Name), "Название игры обязательно");

            if (game.MinPlayers <= 0 || game.MaxPlayers <= 0)
                ModelState.AddModelError(nameof(game.MinPlayers), "Количество игроков должно быть больше нуля");

            if (game.MaxPlayers < game.MinPlayers)
                ModelState.AddModelError(nameof(game.MaxPlayers), "Максимальное количество игроков должно быть больше минимального");

            if (game.YearPublished.HasValue)
            {
                var currentYear = DateTime.UtcNow.Year;
                if (game.YearPublished.Value < 1900 || game.YearPublished.Value > currentYear + 5)
                {
                    ModelState.AddModelError(nameof(game.YearPublished),
                        $"Год издания должен быть между 1900 и {currentYear + 5}");
                }
            }
        }

        private void ValidateEquipmentQuantities(List<int>? selectedEquipment, Dictionary<int, int>? equipmentQuantities)
        {
            if (selectedEquipment == null || !selectedEquipment.Any())
            {
                return;
            }

            foreach (var equipmentId in selectedEquipment.Distinct())
            {
                var fieldKey = $"equipmentQuantities[{equipmentId}]";

                if (equipmentQuantities == null || !equipmentQuantities.TryGetValue(equipmentId, out var quantity))
                {
                    ModelState.AddModelError(fieldKey, "Укажите требуемое количество оборудования.");
                    continue;
                }

                if (quantity <= 0)
                {
                    ModelState.AddModelError(fieldKey, "Требуемое количество должно быть больше нуля.");
                    continue;
                }

                if (quantity > 1000)
                {
                    ModelState.AddModelError(fieldKey, "Требуемое количество не должно превышать 1000.");
                }
            }
        }
    }
}
