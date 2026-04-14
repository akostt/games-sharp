using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GamesSharp.Data;
using GamesSharp.Models;
using GamesSharp.Helpers;

namespace GamesSharp.Controllers
{
    public class GameSessionsController : BaseController
    {
        public GameSessionsController(ApplicationDbContext context, ILogger<GameSessionsController> logger)
            : base(context, logger)
        {
        }

        // GET: GameSessions
        public async Task<IActionResult> Index()
        {
            try
            {
                var gameSessions = await Context.GameSessions
                    .Include(g => g.Game)
                    .Include(g => g.Venue)
                    .Include(g => g.SessionPlayers)
                        .ThenInclude(sp => sp.Player)
                    .AsNoTracking()
                    .OrderByDescending(g => g.ScheduledDate)
                    .ToListAsync();

                return View(gameSessions);
            }
            catch (Exception ex)
            {
                return HandleException(ex, nameof(Index));
            }
        }

        // GET: GameSessions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!IsValidId(id))
                return NotFoundWithLogging("Игровая сессия", id);

            try
            {
                var gameSession = await Context.GameSessions
                    .Include(g => g.Game)
                    .Include(g => g.Venue)
                    .Include(g => g.SessionPlayers)
                        .ThenInclude(sp => sp.Player)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (gameSession == null)
                    return NotFoundWithLogging("Игровая сессия", id);

                return View(gameSession);
            }
            catch (Exception ex)
            {
                return HandleException(ex, nameof(Details));
            }
        }

        // GET: GameSessions/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                await PopulateSelectionDataAsync();
                return View();
            }
            catch (Exception ex)
            {
                return HandleException(ex, nameof(Create));
            }
        }

        // POST: GameSessions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,GameId,VenueId,ScheduledDate,ActualStartTime,ActualEndTime,Notes,Organizer,MaxParticipants")] GameSession gameSession, int[]? selectedPlayers)
        {
            ValidateSessionInput(gameSession, selectedPlayers);

            if (ModelState.IsValid)
            {
                try
                {
                    NormalizeSessionTimes(gameSession);
                    gameSession.Status = Constants.SessionStatus.Scheduled;

                    Context.GameSessions.Add(gameSession);
                    await Context.SaveChangesAsync();

                    await ReplaceSessionPlayersAsync(gameSession.Id, selectedPlayers);
                    await Context.SaveChangesAsync();

                    SetSuccessMessage(Constants.SuccessMessages.RecordCreated);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    return HandleException(ex, nameof(Create));
                }
            }

            await PopulateSelectionDataAsync(gameSession.GameId, gameSession.VenueId, selectedPlayers);
            return View(gameSession);
        }

        // GET: GameSessions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsValidId(id))
                return NotFoundWithLogging("Игровая сессия", id);

            try
            {
                var gameSession = await Context.GameSessions
                    .Include(g => g.SessionPlayers)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(g => g.Id == id);

                if (gameSession == null)
                    return NotFoundWithLogging("Игровая сессия", id);

                await PopulateSelectionDataAsync(
                    gameSession.GameId,
                    gameSession.VenueId,
                    gameSession.SessionPlayers.Select(sp => sp.PlayerId));

                return View(gameSession);
            }
            catch (Exception ex)
            {
                return HandleException(ex, nameof(Edit));
            }
        }

        // POST: GameSessions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,GameId,VenueId,ScheduledDate,ActualStartTime,ActualEndTime,Status,Notes,Organizer,MaxParticipants")] GameSession gameSession, int[]? selectedPlayers)
        {
            if (id != gameSession.Id)
            {
                return NotFoundWithLogging("Игровая сессия", id);
            }

            ValidateSessionInput(gameSession, selectedPlayers);

            if (ModelState.IsValid)
            {
                try
                {
                    NormalizeSessionTimes(gameSession);

                    Context.GameSessions.Update(gameSession);
                    await ReplaceSessionPlayersAsync(gameSession.Id, selectedPlayers);
                    await Context.SaveChangesAsync();

                    SetSuccessMessage(Constants.SuccessMessages.RecordUpdated);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await GameSessionExistsAsync(gameSession.Id))
                    {
                        return NotFoundWithLogging("Игровая сессия", gameSession.Id);
                    }

                    SetErrorMessage(Constants.ErrorMessages.ConcurrencyError);
                    await PopulateSelectionDataAsync(gameSession.GameId, gameSession.VenueId, selectedPlayers);
                    return View(gameSession);
                }

                return RedirectToAction(nameof(Index));
            }

            await PopulateSelectionDataAsync(gameSession.GameId, gameSession.VenueId, selectedPlayers);
            return View(gameSession);
        }

        // GET: GameSessions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsValidId(id))
                return NotFoundWithLogging("Игровая сессия", id);

            try
            {
                var gameSession = await Context.GameSessions
                    .Include(g => g.Game)
                    .Include(g => g.SessionPlayers)
                        .ThenInclude(sp => sp.Player)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (gameSession == null)
                    return NotFoundWithLogging("Игровая сессия", id);

                return View(gameSession);
            }
            catch (Exception ex)
            {
                return HandleException(ex, nameof(Delete));
            }
        }

        // POST: GameSessions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var gameSession = await Context.GameSessions.FindAsync(id);
                if (gameSession == null)
                    return NotFoundWithLogging("Игровая сессия", id);

                Context.GameSessions.Remove(gameSession);
                await Context.SaveChangesAsync();

                SetSuccessMessage(Constants.SuccessMessages.RecordDeleted);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return HandleException(ex, nameof(DeleteConfirmed));
            }
        }

        private async Task PopulateSelectionDataAsync(
            int? selectedGameId = null,
            int? selectedVenueId = null,
            IEnumerable<int>? selectedPlayers = null)
        {
            var games = await Context.Games
                .AsNoTracking()
                .OrderBy(g => g.Name)
                .ToListAsync();

            var venues = await Context.Venues
                .AsNoTracking()
                .OrderBy(v => v.Name)
                .ToListAsync();

            var players = await Context.Players
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .ToListAsync();

            ViewData["GameId"] = new SelectList(games, "Id", "Name", selectedGameId);
            ViewData["VenueId"] = new SelectList(venues, "Id", "Name", selectedVenueId);
            ViewData["Players"] = new MultiSelectList(players, "Id", "Name", selectedPlayers ?? Enumerable.Empty<int>());
        }

        private static void NormalizeSessionTimes(GameSession gameSession)
        {
            if (gameSession.ActualStartTime.HasValue)
            {
                gameSession.ActualStartTime = gameSession.ScheduledDate.Date
                    .Add(gameSession.ActualStartTime.Value.TimeOfDay);
            }

            if (gameSession.ActualEndTime.HasValue)
            {
                gameSession.ActualEndTime = gameSession.ScheduledDate.Date
                    .Add(gameSession.ActualEndTime.Value.TimeOfDay);
            }
        }

        private async Task ReplaceSessionPlayersAsync(int gameSessionId, IEnumerable<int>? selectedPlayers)
        {
            var existingPlayers = await Context.SessionPlayers
                .Where(sp => sp.GameSessionId == gameSessionId)
                .ToListAsync();

            Context.SessionPlayers.RemoveRange(existingPlayers);

            var uniquePlayers = (selectedPlayers ?? Enumerable.Empty<int>()).Distinct();
            foreach (var playerId in uniquePlayers)
            {
                Context.SessionPlayers.Add(new SessionPlayer
                {
                    GameSessionId = gameSessionId,
                    PlayerId = playerId
                });
            }
        }

        private Task<bool> GameSessionExistsAsync(int id)
        {
            return Context.GameSessions.AnyAsync(e => e.Id == id);
        }

        private void ValidateSessionInput(GameSession gameSession, IEnumerable<int>? selectedPlayers)
        {
            if (gameSession.ActualStartTime.HasValue && gameSession.ActualEndTime.HasValue &&
                gameSession.ActualEndTime.Value < gameSession.ActualStartTime.Value)
            {
                ModelState.AddModelError(nameof(gameSession.ActualEndTime),
                    "Время завершения не может быть раньше времени начала");
            }

            if (gameSession.MaxParticipants.HasValue && gameSession.MaxParticipants <= 0)
            {
                ModelState.AddModelError(nameof(gameSession.MaxParticipants),
                    "Максимальное количество участников должно быть больше нуля");
            }

            var selectedCount = selectedPlayers?.Distinct().Count() ?? 0;
            if (gameSession.MaxParticipants.HasValue && selectedCount > gameSession.MaxParticipants.Value)
            {
                ModelState.AddModelError(nameof(gameSession.MaxParticipants),
                    "Количество выбранных участников не может превышать ограничение сессии");
            }
        }
    }
}
