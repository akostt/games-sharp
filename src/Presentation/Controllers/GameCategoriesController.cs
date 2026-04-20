using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GamesSharp.Data;
using GamesSharp.Models;
using GamesSharp.Helpers;

namespace GamesSharp.Controllers
{
    public class GameCategoriesController : BaseController
    {
        public GameCategoriesController(ApplicationDbContext context, ILogger<GameCategoriesController> logger)
            : base(context, logger)
        {
        }

        // GET: GameCategories
        public async Task<IActionResult> Index()
        {
            try
            {
                var categories = await Context.GameCategories
                    .Include(c => c.GameCategoryAssignments)
                    .ToListAsync();
                
                Logger.LogInformation("Загружено {Count} категорий", categories.Count);
                return View(categories);
            }
            catch (Exception ex)
            {
                return HandleException(ex, "Index");
            }
        }

        // GET: GameCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!IsValidId(id))
                return NotFoundWithLogging("Категория", id);

            try
            {
                var category = await Context.GameCategories
                    .Include(c => c.GameCategoryAssignments)
                    .FirstOrDefaultAsync(m => m.Id == id);
                
                if (category == null)
                    return NotFoundWithLogging("Категория", id);

                return View(category);
            }
            catch (Exception ex)
            {
                return HandleException(ex, "Details");
            }
        }

        // GET: GameCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: GameCategories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description")] GameCategory category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Context.Add(category);
                    await Context.SaveChangesAsync();
                    
                    Logger.LogInformation("Создана категория: {CategoryName} (ID: {CategoryId})", category.Name, category.Id);
                    SetSuccessMessage(Constants.SuccessMessages.RecordCreated);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Ошибка при создании категории");
                    SetErrorMessage(Constants.ErrorMessages.DatabaseError);
                }
            }
            
            return View(category);
        }

        // GET: GameCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsValidId(id))
                return NotFoundWithLogging("Категория", id);

            try
            {
                var category = await Context.GameCategories.FindAsync(id);
                if (category == null)
                    return NotFoundWithLogging("Категория", id);

                return View(category);
            }
            catch (Exception ex)
            {
                return HandleException(ex, "Edit");
            }
        }

        // POST: GameCategories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] GameCategory category)
        {
            if (id != category.Id)
                return NotFoundWithLogging("Категория", id);

            if (ModelState.IsValid)
            {
                try
                {
                    Context.Update(category);
                    await Context.SaveChangesAsync();
                    
                    Logger.LogInformation("Обновлена категория: {CategoryName} (ID: {CategoryId})", category.Name, category.Id);
                    SetSuccessMessage(Constants.SuccessMessages.RecordUpdated);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!await CategoryExistsAsync(category.Id))
                    {
                        return NotFoundWithLogging("Категория", category.Id);
                    }
                    else
                    {
                        Logger.LogError(ex, "Ошибка конкурентности при обновлении категории ID: {CategoryId}", category.Id);
                        SetErrorMessage("Запись была изменена другим пользователем");
                    }
                }
                catch (Exception ex)
                {
                    return HandleException(ex, "Edit");
                }
            }
            
            return View(category);
        }

        // GET: GameCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsValidId(id))
                return NotFoundWithLogging("Категория", id);

            try
            {
                var category = await Context.GameCategories
                    .Include(c => c.GameCategoryAssignments)
                    .FirstOrDefaultAsync(m => m.Id == id);
                
                if (category == null)
                    return NotFoundWithLogging("Категория", id);

                return View(category);
            }
            catch (Exception ex)
            {
                return HandleException(ex, "Delete");
            }
        }

        // POST: GameCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var category = await Context.GameCategories.FindAsync(id);
                if (category != null)
                {
                    Context.GameCategories.Remove(category);
                    await Context.SaveChangesAsync();
                    
                    Logger.LogInformation("Удалена категория: {CategoryName} (ID: {CategoryId})", category.Name, category.Id);
                    SetSuccessMessage(Constants.SuccessMessages.RecordDeleted);
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return HandleException(ex, "DeleteConfirmed");
            }
        }

        private async Task<bool> CategoryExistsAsync(int id)
        {
            return await Context.GameCategories.AnyAsync(e => e.Id == id);
        }
    }
}
