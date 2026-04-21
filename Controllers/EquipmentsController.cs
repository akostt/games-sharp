using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GamesSharp.Data;
using GamesSharp.Models;
using GamesSharp.Helpers;

namespace GamesSharp.Controllers
{
    public class EquipmentsController : BaseController
    {
        public EquipmentsController(ApplicationDbContext context, ILogger<EquipmentsController> logger)
            : base(context, logger)
        {
        }

        // GET: Equipments
        public async Task<IActionResult> Index()
        {
            try
            {
                var equipments = await Context.Equipments
                    .Include(e => e.EquipmentType)
                    .AsNoTracking()
                    .OrderBy(e => e.Name)
                    .ToListAsync();
                Logger.LogInformation("Загружено {Count} единиц оборудования", equipments.Count);
                return View(equipments);
            }
            catch (Exception ex)
            {
                return HandleException(ex, "Index");
            }
        }

        // GET: Equipments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!IsValidId(id))
                return NotFoundWithLogging("Оборудование", id);

            try
            {
                var equipment = await Context.Equipments
                    .Include(e => e.EquipmentType)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (equipment == null)
                    return NotFoundWithLogging("Оборудование", id);

                return View(equipment);
            }
            catch (Exception ex)
            {
                return HandleException(ex, "Details");
            }
        }

        // GET: Equipments/Create
        public async Task<IActionResult> Create()
        {
            await PopulateEquipmentTypesAsync();
            return View();
        }

        // POST: Equipments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,EquipmentTypeId,Description")] Equipment equipment)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Context.Add(equipment);
                    await Context.SaveChangesAsync();

                    Logger.LogInformation("Создано оборудование: {EquipmentName} (ID: {EquipmentId})", equipment.Name, equipment.Id);
                    SetSuccessMessage(Constants.SuccessMessages.RecordCreated);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Ошибка при создании оборудования");
                    SetErrorMessage(Constants.ErrorMessages.DatabaseError);
                }
            }

            await PopulateEquipmentTypesAsync(equipment.EquipmentTypeId);
            return View(equipment);
        }

        // GET: Equipments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsValidId(id))
                return NotFoundWithLogging("Оборудование", id);

            try
            {
                var equipment = await Context.Equipments.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
                if (equipment == null)
                    return NotFoundWithLogging("Оборудование", id);

                await PopulateEquipmentTypesAsync(equipment.EquipmentTypeId);

                return View(equipment);
            }
            catch (Exception ex)
            {
                return HandleException(ex, "Edit");
            }
        }

        // POST: Equipments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,EquipmentTypeId,Description")] Equipment equipment)
        {
            if (id != equipment.Id)
                return NotFoundWithLogging("Оборудование", id);

            if (ModelState.IsValid)
            {
                try
                {
                    Context.Update(equipment);
                    await Context.SaveChangesAsync();

                    Logger.LogInformation("Обновлено оборудование: {EquipmentName} (ID: {EquipmentId})", equipment.Name, equipment.Id);
                    SetSuccessMessage(Constants.SuccessMessages.RecordUpdated);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!await EquipmentExistsAsync(equipment.Id))
                    {
                        return NotFoundWithLogging("Оборудование", equipment.Id);
                    }
                    else
                    {
                        Logger.LogError(ex, "Ошибка конкурентности при обновлении оборудования ID: {EquipmentId}", equipment.Id);
                        SetErrorMessage("Запись была изменена другим пользователем");
                    }
                }
                catch (Exception ex)
                {
                    return HandleException(ex, "Edit");
                }
            }

            await PopulateEquipmentTypesAsync(equipment.EquipmentTypeId);
            return View(equipment);
        }

        // GET: Equipments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsValidId(id))
                return NotFoundWithLogging("Оборудование", id);

            try
            {
                var equipment = await Context.Equipments
                    .Include(e => e.EquipmentType)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (equipment == null)
                    return NotFoundWithLogging("Оборудование", id);

                return View(equipment);
            }
            catch (Exception ex)
            {
                return HandleException(ex, "Delete");
            }
        }

        // POST: Equipments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var equipment = await Context.Equipments.FindAsync(id);
                if (equipment != null)
                {
                    Context.Equipments.Remove(equipment);
                    await Context.SaveChangesAsync();

                    Logger.LogInformation("Удалено оборудование: {EquipmentName} (ID: {EquipmentId})", equipment.Name, equipment.Id);
                    SetSuccessMessage(Constants.SuccessMessages.RecordDeleted);
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return HandleException(ex, "DeleteConfirmed");
            }
        }

        private async Task<bool> EquipmentExistsAsync(int id)
        {
            return await Context.Equipments.AnyAsync(e => e.Id == id);
        }

        private async Task PopulateEquipmentTypesAsync(int? selectedTypeId = null)
        {
            var equipmentTypes = await Context.EquipmentTypes
                .AsNoTracking()
                .OrderBy(et => et.Name)
                .ToListAsync();

            ViewData["EquipmentTypeId"] = new SelectList(equipmentTypes, "Id", "Name", selectedTypeId);
        }
    }
}
