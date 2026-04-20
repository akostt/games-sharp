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
                        .ThenInclude(gs => gs.SessionStatus)
                    .Include(v => v.VenueEquipments)
                        .ThenInclude(ve => ve.Equipment)
                            .ThenInclude(e => e.EquipmentType)
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
                    .Include(v => v.GameSessions)
                        .ThenInclude(gs => gs.SessionStatus)
                    .Include(v => v.VenueEquipments)
                        .ThenInclude(ve => ve.Equipment)
                            .ThenInclude(e => e.EquipmentType)
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
            PopulateEquipmentViewData();
            return View();
        }

        // POST: Venues/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Address,Capacity,Phone,RentalCostPerHour,Description,Latitude,Longitude")] Venue venue, Dictionary<int, int>? equipmentQuantities)
        {
            await ValidateVenueEquipmentQuantitiesAsync(equipmentQuantities);

            if (ModelState.IsValid)
            {
                try
                {
                    Context.Add(venue);
                    await Context.SaveChangesAsync();

                    await SaveVenueEquipmentAsync(venue.Id, equipmentQuantities);
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

            PopulateEquipmentViewData(equipmentQuantities);
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

                var equipmentQuantities = await Context.VenueEquipments
                    .AsNoTracking()
                    .Where(ve => ve.VenueId == venue.Id)
                    .ToDictionaryAsync(ve => ve.EquipmentId, ve => ve.Quantity);

                PopulateEquipmentViewData(equipmentQuantities);
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,Capacity,Phone,RentalCostPerHour,Description,Latitude,Longitude")] Venue venue, Dictionary<int, int>? equipmentQuantities)
        {
            if (id != venue.Id)
                return NotFoundWithLogging("Место проведения", id);

            await ValidateVenueEquipmentQuantitiesAsync(equipmentQuantities);

            if (ModelState.IsValid)
            {
                try
                {
                    Context.Update(venue);

                    var existingEquipment = await Context.VenueEquipments
                        .Where(ve => ve.VenueId == venue.Id)
                        .ToListAsync();

                    Context.VenueEquipments.RemoveRange(existingEquipment);
                    await SaveVenueEquipmentAsync(venue.Id, equipmentQuantities);

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

            PopulateEquipmentViewData(equipmentQuantities);
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

        private void PopulateEquipmentViewData(Dictionary<int, int>? equipmentQuantities = null)
        {
            ViewBag.AllEquipment = Context.Equipments
                .Include(e => e.EquipmentType)
                .AsNoTracking()
                .OrderBy(e => e.Name)
                .ToList();

            ViewBag.EquipmentQuantities = equipmentQuantities ?? new Dictionary<int, int>();
        }

        private Task SaveVenueEquipmentAsync(int venueId, Dictionary<int, int>? equipmentQuantities)
        {
            if (equipmentQuantities == null)
            {
                return Task.CompletedTask;
            }

            var items = equipmentQuantities
                .Where(kv => kv.Value > 0)
                .Select(kv => new VenueEquipment
                {
                    VenueId = venueId,
                    EquipmentId = kv.Key,
                    Quantity = kv.Value
                })
                .ToList();

            if (items.Count > 0)
            {
                Context.VenueEquipments.AddRange(items);
            }

            return Task.CompletedTask;
        }

        private async Task ValidateVenueEquipmentQuantitiesAsync(Dictionary<int, int>? equipmentQuantities)
        {
            if (equipmentQuantities == null || equipmentQuantities.Count == 0)
            {
                return;
            }

            var validEquipmentIds = await Context.Equipments
                .AsNoTracking()
                .Select(e => e.Id)
                .ToListAsync();

            var validSet = validEquipmentIds.ToHashSet();

            foreach (var pair in equipmentQuantities)
            {
                var fieldKey = $"equipmentQuantities[{pair.Key}]";

                if (!validSet.Contains(pair.Key))
                {
                    ModelState.AddModelError(fieldKey, "Выбрано неизвестное оборудование.");
                    continue;
                }

                if (pair.Value < 0)
                {
                    ModelState.AddModelError(fieldKey, "Количество на площадке не может быть отрицательным.");
                }

                if (pair.Value > 10000)
                {
                    ModelState.AddModelError(fieldKey, "Количество на площадке не должно превышать 10000.");
                }
            }
        }
    }
}
