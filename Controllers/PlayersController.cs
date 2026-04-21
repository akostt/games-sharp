using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GamesSharp.Data;
using GamesSharp.Models;
using GamesSharp.Helpers;
using GamesSharp.Services;

namespace GamesSharp.Controllers
{
    public class PlayersController : BaseController
    {
        private readonly IExcelExportService _excelExportService;

        public PlayersController(
            ApplicationDbContext context,
            ILogger<PlayersController> logger,
            IExcelExportService excelExportService)
            : base(context, logger)
        {
            _excelExportService = excelExportService ?? throw new ArgumentNullException(nameof(excelExportService));
        }

        // GET: Players
        public async Task<IActionResult> Index()
        {
            try
            {
                var players = await Context.Players
                    .AsNoTracking()
                    .OrderBy(p => p.Name)
                    .ToListAsync();

                return View(players);
            }
            catch (Exception ex)
            {
                return HandleException(ex, nameof(Index));
            }
        }

        // GET: Players/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!IsValidId(id))
                return NotFoundWithLogging("Игрок", id);

            try
            {
                var player = await Context.Players
                    .Include(p => p.SessionPlayers)
                        .ThenInclude(sp => sp.GameSession)
                            .ThenInclude(gs => gs.Game)
                    .Include(p => p.SessionPlayers)
                        .ThenInclude(sp => sp.GameSession)
                            .ThenInclude(gs => gs.SessionStatus)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (player == null)
                    return NotFoundWithLogging("Игрок", id);

                return View(player);
            }
            catch (Exception ex)
            {
                return HandleException(ex, nameof(Details));
            }
        }

        // GET: Players/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Players/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,Phone,BirthDate,City,FavoriteGenre")] Player player)
        {
            ValidatePlayerData(player);

            if (ModelState.IsValid)
            {
                try
                {
                    player.RegisteredDate = DateTime.Now;
                    Context.Players.Add(player);
                    await Context.SaveChangesAsync();

                    SetSuccessMessage(Constants.SuccessMessages.RecordCreated);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    return HandleException(ex, nameof(Create));
                }
            }

            return View(player);
        }

        // GET: Players/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsValidId(id))
                return NotFoundWithLogging("Игрок", id);

            try
            {
                var player = await Context.Players.FindAsync(id);
                if (player == null)
                    return NotFoundWithLogging("Игрок", id);

                return View(player);
            }
            catch (Exception ex)
            {
                return HandleException(ex, nameof(Edit));
            }
        }

        // POST: Players/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Phone,BirthDate,City,FavoriteGenre")] Player player)
        {
            if (id != player.Id)
            {
                return NotFoundWithLogging("Игрок", id);
            }

            ValidatePlayerData(player);

            if (ModelState.IsValid)
            {
                try
                {
                    var existingPlayer = await Context.Players.FirstOrDefaultAsync(p => p.Id == id);
                    if (existingPlayer == null)
                    {
                        return NotFoundWithLogging("Игрок", id);
                    }

                    existingPlayer.Name = player.Name;
                    existingPlayer.Email = player.Email;
                    existingPlayer.Phone = player.Phone;
                    existingPlayer.BirthDate = player.BirthDate;
                    existingPlayer.City = player.City;
                    existingPlayer.FavoriteGenre = player.FavoriteGenre;

                    await Context.SaveChangesAsync();

                    SetSuccessMessage(Constants.SuccessMessages.RecordUpdated);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await PlayerExistsAsync(player.Id))
                    {
                        return NotFoundWithLogging("Игрок", player.Id);
                    }

                    SetErrorMessage(Constants.ErrorMessages.ConcurrencyError);
                    return View(player);
                }

                return RedirectToAction(nameof(Index));
            }

            return View(player);
        }

        // GET: Players/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsValidId(id))
                return NotFoundWithLogging("Игрок", id);

            try
            {
                var player = await Context.Players
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (player == null)
                    return NotFoundWithLogging("Игрок", id);

                return View(player);
            }
            catch (Exception ex)
            {
                return HandleException(ex, nameof(Delete));
            }
        }

        // GET: Players/ExportToExcel
        [HttpGet]
        public async Task<IActionResult> ExportToExcel(CancellationToken cancellationToken)
        {
            try
            {
                var players = await Context.Players
                    .AsNoTracking()
                    .OrderBy(p => p.Name)
                    .ToListAsync(cancellationToken);

                var exportResult = await _excelExportService.ExportPlayersAsync(players, cancellationToken);

                Logger.LogInformation("Сформирован экспорт игроков в Excel. Записей: {Count}", players.Count);
                return PhysicalFile(exportResult.StoredFilePath, exportResult.ContentType, exportResult.DownloadFileName);
            }
            catch (Exception ex)
            {
                return HandleException(ex, nameof(ExportToExcel));
            }
        }

        // POST: Players/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var player = await Context.Players.FindAsync(id);
                if (player == null)
                    return NotFoundWithLogging("Игрок", id);

                Context.Players.Remove(player);
                await Context.SaveChangesAsync();

                SetSuccessMessage(Constants.SuccessMessages.RecordDeleted);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return HandleException(ex, nameof(DeleteConfirmed));
            }
        }

        private Task<bool> PlayerExistsAsync(int id)
        {
            return Context.Players.AnyAsync(e => e.Id == id);
        }

        private void ValidatePlayerData(Player player)
        {
            if (string.IsNullOrWhiteSpace(player.Name))
            {
                ModelState.AddModelError(nameof(player.Name), "Имя игрока обязательно");
            }

            if (player.BirthDate.HasValue && player.BirthDate.Value.Date > DateTime.Today)
            {
                ModelState.AddModelError(nameof(player.BirthDate), "Дата рождения не может быть в будущем");
            }
        }
    }
}
