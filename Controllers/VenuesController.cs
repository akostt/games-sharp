using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GamesSharp.Data;
using GamesSharp.Models;
using GamesSharp.Helpers;

namespace GamesSharp.Controllers
{
    public class VenuesController : BaseController
    {
        public VenuesController(ApplicationDbContext context, ILogger<VenuesController> logger)
            : base(context, logger)
        {
        }

        // GET: Venues
        public async Task<IActionResult> Index()
        {
            try
            {
                var venues = await Context.Venues
                    .Include(v => v.GameSessions)
                    .ToListAsync();

                Logger.LogInformation("Загружено {Count} мест проведения", venues.Count);
                return View(venues);
            }
            catch (Exception ex)
            {
                return HandleException(ex, "Index");
            }
        }

        // GET: Venues/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!IsValidId(id))
                return NotFoundWithLogging("Место проведения", id);

            try
            {
                var venue = await Context.Venues
                    .Include(v => v.GameSessions)
                    .ThenInclude(gs => gs.Game)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (venue == null)
                    return NotFoundWithLogging("Место проведения", id);

                return View(venue);
            }
            catch (Exception ex)
            {
                return HandleException(ex, "Details");
            }
        }

        // GET: Venues/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Venues/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Address,Capacity,Phone,RentalCostPerHour,Description")] Venue venue)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Context.Add(venue);
                    await Context.SaveChangesAsync();

                    Logger.LogInformation("Создано место: {VenueName} (ID: {VenueId})", venue.Name, venue.Id);
                    SetSuccessMessage(Constants.SuccessMessages.RecordCreated);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Ошибка при создании места проведения");
                    SetErrorMessage(Constants.ErrorMessages.DatabaseError);
                }
            }

            return View(venue);
        }

        // GET: Venues/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsValidId(id))
                return NotFoundWithLogging("Место проведения", id);

            try
            {
                var venue = await Context.Venues.FindAsync(id);
                if (venue == null)
                    return NotFoundWithLogging("Место проведения", id);

                return View(venue);
            }
            catch (Exception ex)
            {
                return HandleException(ex, "Edit");
            }
        }

        // POST: Venues/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,Capacity,Phone,RentalCostPerHour,Description")] Venue venue)
        {
            if (id != venue.Id)
                return NotFoundWithLogging("Место проведения", id);

            if (ModelState.IsValid)
            {
                try
                {
                    Context.Update(venue);
                    await Context.SaveChangesAsync();

                    Logger.LogInformation("Обновлено место: {VenueName} (ID: {VenueId})", venue.Name, venue.Id);
                    SetSuccessMessage(Constants.SuccessMessages.RecordUpdated);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!await VenueExistsAsync(venue.Id))
                    {
                        return NotFoundWithLogging("Место проведения", venue.Id);
                    }
                    else
                    {
                        Logger.LogError(ex, "Ошибка конкурентности при обновлении места ID: {VenueId}", venue.Id);
                        SetErrorMessage("Запись была изменена другим пользователем");
                    }
                }
                catch (Exception ex)
                {
                    return HandleException(ex, "Edit");
                }
            }

            return View(venue);
        }

        // GET: Venues/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsValidId(id))
                return NotFoundWithLogging("Место проведения", id);

            try
            {
                var venue = await Context.Venues
                    .Include(v => v.GameSessions)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (venue == null)
                    return NotFoundWithLogging("Место проведения", id);

                return View(venue);
            }
            catch (Exception ex)
            {
                return HandleException(ex, "Delete");
            }
        }

        // POST: Venues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var venue = await Context.Venues.FindAsync(id);
                if (venue != null)
                {
                    Context.Venues.Remove(venue);
                    await Context.SaveChangesAsync();

                    Logger.LogInformation("Удалено место: {VenueName} (ID: {VenueId})", venue.Name, venue.Id);
                    SetSuccessMessage(Constants.SuccessMessages.RecordDeleted);
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return HandleException(ex, "DeleteConfirmed");
            }
        }

        private async Task<bool> VenueExistsAsync(int id)
        {
            return await Context.Venues.AnyAsync(e => e.Id == id);
        }
    }
}
