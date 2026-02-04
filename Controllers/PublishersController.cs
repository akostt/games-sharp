using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GamesSharp.Data;
using GamesSharp.Models;
using GamesSharp.Helpers;

namespace GamesSharp.Controllers
{
    public class PublishersController : BaseController
    {
        public PublishersController(ApplicationDbContext context, ILogger<PublishersController> logger)
            : base(context, logger)
        {
        }

        // GET: Publishers
        public async Task<IActionResult> Index()
        {
            try
            {
                var publishers = await Context.Publishers
                    .Include(p => p.Games)
                    .ToListAsync();
                
                Logger.LogInformation("Загружено {Count} издателей", publishers.Count);
                return View(publishers);
            }
            catch (Exception ex)
            {
                return HandleException(ex, "Index");
            }
        }

        // GET: Publishers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!IsValidId(id))
                return NotFoundWithLogging("Издатель", id);

            try
            {
                var publisher = await Context.Publishers
                    .Include(p => p.Games)
                    .FirstOrDefaultAsync(m => m.Id == id);
                
                if (publisher == null)
                    return NotFoundWithLogging("Издатель", id);

                return View(publisher);
            }
            catch (Exception ex)
            {
                return HandleException(ex, "Details");
            }
        }

        // GET: Publishers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Publishers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Country,FoundedYear,Website")] Publisher publisher)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Context.Add(publisher);
                    await Context.SaveChangesAsync();
                    
                    Logger.LogInformation("Создан издатель: {PublisherName} (ID: {PublisherId})", publisher.Name, publisher.Id);
                    SetSuccessMessage(Constants.SuccessMessages.RecordCreated);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Ошибка при создании издателя");
                    SetErrorMessage(Constants.ErrorMessages.DatabaseError);
                }
            }
            
            return View(publisher);
        }

        // GET: Publishers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsValidId(id))
                return NotFoundWithLogging("Издатель", id);

            try
            {
                var publisher = await Context.Publishers.FindAsync(id);
                if (publisher == null)
                    return NotFoundWithLogging("Издатель", id);

                return View(publisher);
            }
            catch (Exception ex)
            {
                return HandleException(ex, "Edit");
            }
        }

        // POST: Publishers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Country,FoundedYear,Website")] Publisher publisher)
        {
            if (id != publisher.Id)
                return NotFoundWithLogging("Издатель", id);

            if (ModelState.IsValid)
            {
                try
                {
                    Context.Update(publisher);
                    await Context.SaveChangesAsync();
                    
                    Logger.LogInformation("Обновлен издатель: {PublisherName} (ID: {PublisherId})", publisher.Name, publisher.Id);
                    SetSuccessMessage(Constants.SuccessMessages.RecordUpdated);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!await PublisherExistsAsync(publisher.Id))
                    {
                        return NotFoundWithLogging("Издатель", publisher.Id);
                    }
                    else
                    {
                        Logger.LogError(ex, "Ошибка конкурентности при обновлении издателя ID: {PublisherId}", publisher.Id);
                        SetErrorMessage("Запись была изменена другим пользователем");
                    }
                }
                catch (Exception ex)
                {
                    return HandleException(ex, "Edit");
                }
            }
            
            return View(publisher);
        }

        // GET: Publishers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsValidId(id))
                return NotFoundWithLogging("Издатель", id);

            try
            {
                var publisher = await Context.Publishers
                    .Include(p => p.Games)
                    .FirstOrDefaultAsync(m => m.Id == id);
                
                if (publisher == null)
                    return NotFoundWithLogging("Издатель", id);

                return View(publisher);
            }
            catch (Exception ex)
            {
                return HandleException(ex, "Delete");
            }
        }

        // POST: Publishers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var publisher = await Context.Publishers.FindAsync(id);
                if (publisher != null)
                {
                    Context.Publishers.Remove(publisher);
                    await Context.SaveChangesAsync();
                    
                    Logger.LogInformation("Удален издатель: {PublisherName} (ID: {PublisherId})", publisher.Name, publisher.Id);
                    SetSuccessMessage(Constants.SuccessMessages.RecordDeleted);
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return HandleException(ex, "DeleteConfirmed");
            }
        }

        private async Task<bool> PublisherExistsAsync(int id)
        {
            return await Context.Publishers.AnyAsync(e => e.Id == id);
        }
    }
}
