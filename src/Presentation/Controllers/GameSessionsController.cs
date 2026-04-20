using GamesSharp.Data;
using GamesSharp.Helpers;
using GamesSharp.Models;
using GamesSharp.Services.GameSessions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GamesSharp.Controllers
{
    public class GameSessionsController : BaseController
    {
        private readonly IGameSessionService _gameSessionService;

        public GameSessionsController(
            ApplicationDbContext context,
            ILogger<GameSessionsController> logger,
            IGameSessionService gameSessionService)
            : base(context, logger)
        {
            _gameSessionService = gameSessionService;
        }

        // GET: GameSessions
        public async Task<IActionResult> Index()
        {
            try
            {
                var gameSessions = await _gameSessionService.GetSessionsForIndexAsync();
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
            {
                return NotFoundWithLogging("Игровая сессия", id);
            }

            try
            {
                var sessionId = id.GetValueOrDefault();
                var gameSession = await _gameSessionService.GetSessionDetailsAsync(sessionId);
                if (gameSession == null)
                {
                    return NotFoundWithLogging("Игровая сессия", id);
                }

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
            await AddValidationErrorsToModelStateAsync(gameSession, selectedPlayers);

            if (ModelState.IsValid)
            {
                try
                {
                    await _gameSessionService.CreateSessionAsync(gameSession, selectedPlayers);
                    SetSuccessMessage(Constants.SuccessMessages.RecordCreated);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    return HandleException(ex, nameof(Create));
                }
            }

            await PopulateSelectionDataAsync(gameSession.GameId, gameSession.VenueId, null, selectedPlayers);
            return View(gameSession);
        }

        // GET: GameSessions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsValidId(id))
            {
                return NotFoundWithLogging("Игровая сессия", id);
            }

            try
            {
                var sessionId = id.GetValueOrDefault();
                var gameSession = await _gameSessionService.GetSessionForEditAsync(sessionId);
                if (gameSession == null)
                {
                    return NotFoundWithLogging("Игровая сессия", id);
                }

                await PopulateSelectionDataAsync(
                    gameSession.GameId,
                    gameSession.VenueId,
                    gameSession.SessionStatusId,
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,GameId,VenueId,ScheduledDate,ActualStartTime,ActualEndTime,SessionStatusId,Notes,Organizer,MaxParticipants")] GameSession gameSession, int[]? selectedPlayers)
        {
            if (id != gameSession.Id)
            {
                return NotFoundWithLogging("Игровая сессия", id);
            }

            await AddValidationErrorsToModelStateAsync(gameSession, selectedPlayers);

            if (ModelState.IsValid)
            {
                try
                {
                    await _gameSessionService.UpdateSessionAsync(gameSession, selectedPlayers);
                    SetSuccessMessage(Constants.SuccessMessages.RecordUpdated);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _gameSessionService.ExistsAsync(gameSession.Id))
                    {
                        return NotFoundWithLogging("Игровая сессия", gameSession.Id);
                    }

                    SetErrorMessage(Constants.ErrorMessages.ConcurrencyError);
                }
                catch (Exception ex)
                {
                    return HandleException(ex, nameof(Edit));
                }
            }

            await PopulateSelectionDataAsync(gameSession.GameId, gameSession.VenueId, gameSession.SessionStatusId, selectedPlayers);
            return View(gameSession);
        }

        // GET: GameSessions/Results/5
        public async Task<IActionResult> Results(int? id)
        {
            if (!IsValidId(id))
            {
                return NotFoundWithLogging("Игровая сессия", id);
            }

            try
            {
                var sessionId = id.GetValueOrDefault();
                var model = await _gameSessionService.GetResultsModelAsync(sessionId);
                if (model == null)
                {
                    return NotFoundWithLogging("Игровая сессия", id);
                }

                return View(model);
            }
            catch (Exception ex)
            {
                return HandleException(ex, nameof(Results));
            }
        }

        // POST: GameSessions/Results/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Results(int id, GameSessionResultsViewModel model)
        {
            if (id != model.GameSessionId)
            {
                return NotFoundWithLogging("Игровая сессия", id);
            }

            var submittedPlayers = model.Players ?? new List<SessionPlayerResultInput>();
            if (submittedPlayers.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "Для сессии нет участников, которым можно выставить результат.");
            }

            if (!ModelState.IsValid)
            {
                var summary = await _gameSessionService.GetSessionSummaryAsync(id);
                if (summary != null)
                {
                    model.GameName = summary.GameName;
                    model.ScheduledDate = summary.ScheduledDate;
                }

                return View(model);
            }

            try
            {
                var saved = await _gameSessionService.SaveResultsAsync(id, submittedPlayers);
                if (!saved)
                {
                    return NotFoundWithLogging("Игровая сессия", id);
                }

                SetSuccessMessage("Результаты игры сохранены");
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                return HandleException(ex, nameof(Results));
            }
        }

        // GET: GameSessions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsValidId(id))
            {
                return NotFoundWithLogging("Игровая сессия", id);
            }

            try
            {
                var sessionId = id.GetValueOrDefault();
                var gameSession = await _gameSessionService.GetSessionForDeleteAsync(sessionId);
                if (gameSession == null)
                {
                    return NotFoundWithLogging("Игровая сессия", id);
                }

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
                var deleted = await _gameSessionService.DeleteSessionAsync(id);
                if (!deleted)
                {
                    return NotFoundWithLogging("Игровая сессия", id);
                }

                SetSuccessMessage(Constants.SuccessMessages.RecordDeleted);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return HandleException(ex, nameof(DeleteConfirmed));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetEquipmentAvailability(int gameId, int? venueId)
        {
            if (gameId <= 0 || !venueId.HasValue)
            {
                return Json(Array.Empty<object>());
            }

            var result = await _gameSessionService.GetEquipmentAvailabilityAsync(gameId, venueId.Value);
            return Json(result);
        }

        private async Task AddValidationErrorsToModelStateAsync(GameSession gameSession, IEnumerable<int>? selectedPlayers)
        {
            var validationErrors = await _gameSessionService.ValidateSessionInputAsync(gameSession, selectedPlayers);
            foreach (var error in validationErrors)
            {
                ModelState.AddModelError(error.Key, error.Message);
            }
        }

        private async Task PopulateSelectionDataAsync(
            int? selectedGameId = null,
            int? selectedVenueId = null,
            int? selectedStatusId = null,
            IEnumerable<int>? selectedPlayers = null)
        {
            var selectionData = await _gameSessionService.GetSelectionDataAsync();

            ViewData["GameId"] = new SelectList(selectionData.Games, "Id", "Name", selectedGameId);
            ViewData["VenueId"] = new SelectList(selectionData.Venues, "Id", "Name", selectedVenueId);
            ViewData["SessionStatusId"] = new SelectList(selectionData.Statuses, "Id", "Name", selectedStatusId ?? Constants.SessionStatus.ScheduledId);
            ViewData["Players"] = new MultiSelectList(selectionData.Players, "Id", "Name", selectedPlayers ?? Enumerable.Empty<int>());
        }
    }
}
