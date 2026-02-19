using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GamesSharp.Data;
using GamesSharp.Models;
using GamesSharp.Helpers;

namespace GamesSharp.Controllers
{
    public class GamesController : BaseController
    {
        public GamesController(ApplicationDbContext context, ILogger<GamesController> logger)
            : base(context, logger)
        {
        }

        // GET: Games
        public async Task<IActionResult> Index()
        {
            try
            {
                var games = await Context.Games
                    .Include(g => g.GameCategoryAssignments)
                        .ThenInclude(gca => gca.GameCategory)
                    .Include(g => g.Publisher)
                    .ToListAsync();
                
                Logger.LogInformation("Загружено {Count} игр", games.Count);
                return View(games);
            }
            catch (Exception ex)
            {
                return HandleException(ex, "Index");
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
                    .Include(g => g.GameSessions)
                        .ThenInclude(gs => gs.Venue)
                    .Include(g => g.GameSessions)
                        .ThenInclude(gs => gs.SessionPlayers)
                    .FirstOrDefaultAsync(m => m.Id == id);
                
                if (game == null)
                    return NotFoundWithLogging("Игра", id);

                Logger.LogInformation("Просмотр деталей игры: {GameName} (ID: {GameId})", game.Name, game.Id);
                return View(game);
            }
            catch (Exception ex)
            {
                return HandleException(ex, "Details");
            }
        }

        // GET: Games/Create
        public async Task<IActionResult> Create()
        {
            PopulateCategoryList();
            PopulatePublisherList();
            await PopulateEquipmentList();
            return View();
        }

        // POST: Games/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,MinPlayers,MaxPlayers,AverageDuration,Complexity,MinAge,YearPublished,PublisherId")] Game game, List<int>? selectedCategoryIds, List<int>? selectedEquipment, Dictionary<int, int>? equipmentQuantities)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Context.Add(game);
                    await Context.SaveChangesAsync();

                    if (selectedCategoryIds != null && selectedCategoryIds.Any())
                    {
                        foreach (var categoryId in selectedCategoryIds.Distinct())
                        {
                            Context.GameCategoryAssignments.Add(new GameCategoryAssignment
                            {
                                GameId = game.Id,
                                GameCategoryId = categoryId
                            });
                        }
                        await Context.SaveChangesAsync();
                    }
                    
                    // Add selected equipment
                    if (selectedEquipment != null && selectedEquipment.Any())
                    {
                        foreach (var equipmentId in selectedEquipment)
                        {
                            var quantity = equipmentQuantities != null && equipmentQuantities.ContainsKey(equipmentId) 
                                ? equipmentQuantities[equipmentId] 
                                : 1;
                            
                            var gameEquipment = new GameEquipment
                            {
                                GameId = game.Id,
                                EquipmentId = equipmentId,
                                RequiredQuantity = quantity
                            };
                            Context.GameEquipments.Add(gameEquipment);
                        }
                        await Context.SaveChangesAsync();
                    }
                    
                    Logger.LogInformation("Создана новая игра: {GameName} (ID: {GameId})", game.Name, game.Id);
                    SetSuccessMessage(Constants.SuccessMessages.RecordCreated);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Ошибка при создании игры");
                    SetErrorMessage(Constants.ErrorMessages.DatabaseError);
                }
            }
            
            PopulateCategoryList(selectedCategoryIds);
            PopulatePublisherList(game.PublisherId);
            await PopulateEquipmentList();
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
                    .FirstOrDefaultAsync(g => g.Id == id);
                if (game == null)
                    return NotFoundWithLogging("Игра", id);

                var selectedCategoryIds = game.GameCategoryAssignments
                    .Select(gca => gca.GameCategoryId)
                    .ToList();
                PopulateCategoryList(selectedCategoryIds);
                PopulatePublisherList(game.PublisherId);
                await PopulateEquipmentList(game.Id);
                return View(game);
            }
            catch (Exception ex)
            {
                return HandleException(ex, "Edit");
            }
        }

        // POST: Games/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,MinPlayers,MaxPlayers,AverageDuration,Complexity,MinAge,YearPublished,PublisherId")] Game game, List<int>? selectedCategoryIds, List<int>? selectedEquipment, Dictionary<int, int>? equipmentQuantities)
        {
            if (id != game.Id)
                return NotFoundWithLogging("Игра", id);

            if (ModelState.IsValid)
            {
                try
                {
                    Context.Update(game);
                    await Context.SaveChangesAsync();

                    var existingCategories = Context.GameCategoryAssignments
                        .Where(gca => gca.GameId == game.Id);
                    Context.GameCategoryAssignments.RemoveRange(existingCategories);

                    if (selectedCategoryIds != null && selectedCategoryIds.Any())
                    {
                        foreach (var categoryId in selectedCategoryIds.Distinct())
                        {
                            Context.GameCategoryAssignments.Add(new GameCategoryAssignment
                            {
                                GameId = game.Id,
                                GameCategoryId = categoryId
                            });
                        }
                    }
                    
                    // Remove existing equipment associations
                    var existingEquipment = Context.GameEquipments.Where(ge => ge.GameId == game.Id);
                    Context.GameEquipments.RemoveRange(existingEquipment);
                    
                    // Add new equipment associations
                    if (selectedEquipment != null && selectedEquipment.Any())
                    {
                        foreach (var equipmentId in selectedEquipment)
                        {
                            var quantity = equipmentQuantities != null && equipmentQuantities.ContainsKey(equipmentId) 
                                ? equipmentQuantities[equipmentId] 
                                : 1;
                            
                            var gameEquipment = new GameEquipment
                            {
                                GameId = game.Id,
                                EquipmentId = equipmentId,
                                RequiredQuantity = quantity
                            };
                            Context.GameEquipments.Add(gameEquipment);
                        }
                    }
                    
                    await Context.SaveChangesAsync();
                    
                    Logger.LogInformation("Обновлена игра: {GameName} (ID: {GameId})", game.Name, game.Id);
                    SetSuccessMessage(Constants.SuccessMessages.RecordUpdated);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!await GameExistsAsync(game.Id))
                    {
                        return NotFoundWithLogging("Игра", game.Id);
                    }
                    else
                    {
                        Logger.LogError(ex, "Ошибка конкурентности при обновлении игры ID: {GameId}", game.Id);
                        SetErrorMessage("Запись была изменена другим пользователем");
                    }
                }
                catch (Exception ex)
                {
                    return HandleException(ex, "Edit");
                }
            }
            
            PopulateCategoryList(selectedCategoryIds);
            PopulatePublisherList(game.PublisherId);
            await PopulateEquipmentList(game.Id);
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
                    .FirstOrDefaultAsync(m => m.Id == id);
                
                if (game == null)
                    return NotFoundWithLogging("Игра", id);

                return View(game);
            }
            catch (Exception ex)
            {
                return HandleException(ex, "Delete");
            }
        }

        // POST: Games/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var game = await Context.Games.FindAsync(id);
                if (game != null)
                {
                    Context.Games.Remove(game);
                    await Context.SaveChangesAsync();
                    
                    Logger.LogInformation("Удалена игра: {GameName} (ID: {GameId})", game.Name, game.Id);
                    SetSuccessMessage(Constants.SuccessMessages.RecordDeleted);
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return HandleException(ex, "DeleteConfirmed");
            }
        }

        private async Task<bool> GameExistsAsync(int id)
        {
            return await Context.Games.AnyAsync(e => e.Id == id);
        }

        private void PopulatePublisherList(int? publisherId = null)
        {
            ViewBag.PublisherId = new SelectList(Context.Publishers, "Id", "Name", publisherId);
        }

        private void PopulateCategoryList(List<int>? selectedCategoryIds = null)
        {
            ViewBag.AllCategories = Context.GameCategories.ToList();
            ViewBag.SelectedCategoryIds = selectedCategoryIds ?? new List<int>();
        }

        private async Task PopulateEquipmentList(int? gameId = null)
        {
            var allEquipment = await Context.Equipments.ToListAsync();
            var selectedEquipment = new List<int>();
            var equipmentQuantities = new Dictionary<int, int>();

            if (gameId.HasValue)
            {
                var gameEquipments = await Context.GameEquipments
                    .Where(ge => ge.GameId == gameId.Value)
                    .ToListAsync();
                selectedEquipment = gameEquipments.Select(ge => ge.EquipmentId).ToList();
                equipmentQuantities = gameEquipments.ToDictionary(ge => ge.EquipmentId, ge => ge.RequiredQuantity);
            }

            ViewBag.AllEquipment = allEquipment;
            ViewBag.SelectedEquipment = selectedEquipment;
            ViewBag.EquipmentQuantities = equipmentQuantities;
        }
    }
}
