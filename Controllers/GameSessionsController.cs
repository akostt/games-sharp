using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GamesSharp.Data;
using GamesSharp.Models;

namespace GamesSharp.Controllers
{
    public class GameSessionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GameSessionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: GameSessions
        public async Task<IActionResult> Index()
        {
            var gameSessions = await _context.GameSessions
                .Include(g => g.Game)
                .Include(g => g.Venue)
                .Include(g => g.SessionPlayers)
                    .ThenInclude(sp => sp.Player)
                .OrderByDescending(g => g.ScheduledDate)
                .ToListAsync();
            return View(gameSessions);
        }

        // GET: GameSessions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gameSession = await _context.GameSessions
                .Include(g => g.Game)
                .Include(g => g.Venue)
                .Include(g => g.SessionPlayers)
                    .ThenInclude(sp => sp.Player)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gameSession == null)
            {
                return NotFound();
            }

            return View(gameSession);
        }

        // GET: GameSessions/Create
        public IActionResult Create()
        {
            ViewData["GameId"] = new SelectList(_context.Games, "Id", "Name");
            ViewData["VenueId"] = new SelectList(_context.Venues, "Id", "Name");
            ViewData["Players"] = new MultiSelectList(_context.Players, "Id", "Name");
            return View();
        }

        // POST: GameSessions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,GameId,VenueId,ScheduledDate,ActualStartTime,ActualEndTime,Notes,Organizer,MaxParticipants")] GameSession gameSession, int[] selectedPlayers)
        {
            if (ModelState.IsValid)
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

                gameSession.Status = "Запланирована";
                _context.Add(gameSession);
                await _context.SaveChangesAsync();

                // Add selected players to the session
                if (selectedPlayers != null && selectedPlayers.Length > 0)
                {
                    foreach (var playerId in selectedPlayers)
                    {
                        var sessionPlayer = new SessionPlayer
                        {
                            GameSessionId = gameSession.Id,
                            PlayerId = playerId
                        };
                        _context.SessionPlayers.Add(sessionPlayer);
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            ViewData["GameId"] = new SelectList(_context.Games, "Id", "Name", gameSession.GameId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "Id", "Name", gameSession.VenueId);
            ViewData["Players"] = new MultiSelectList(_context.Players, "Id", "Name", selectedPlayers);
            return View(gameSession);
        }

        // GET: GameSessions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gameSession = await _context.GameSessions
                .Include(g => g.SessionPlayers)
                .FirstOrDefaultAsync(g => g.Id == id);
            if (gameSession == null)
            {
                return NotFound();
            }
            ViewData["GameId"] = new SelectList(_context.Games, "Id", "Name", gameSession.GameId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "Id", "Name", gameSession.VenueId);
            ViewData["Players"] = new MultiSelectList(_context.Players, "Id", "Name", 
                gameSession.SessionPlayers.Select(sp => sp.PlayerId).ToArray());
            return View(gameSession);
        }

        // POST: GameSessions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,GameId,VenueId,ScheduledDate,ActualStartTime,ActualEndTime,Status,Notes,Organizer,MaxParticipants")] GameSession gameSession, int[] selectedPlayers)
        {
            if (id != gameSession.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
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

                    _context.Update(gameSession);
                    
                    // Update session players
                    var existingPlayers = await _context.SessionPlayers
                        .Where(sp => sp.GameSessionId == id)
                        .ToListAsync();
                    _context.SessionPlayers.RemoveRange(existingPlayers);
                    
                    if (selectedPlayers != null && selectedPlayers.Length > 0)
                    {
                        foreach (var playerId in selectedPlayers)
                        {
                            var sessionPlayer = new SessionPlayer
                            {
                                GameSessionId = gameSession.Id,
                                PlayerId = playerId
                            };
                            _context.SessionPlayers.Add(sessionPlayer);
                        }
                    }
                    
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameSessionExists(gameSession.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["GameId"] = new SelectList(_context.Games, "Id", "Name", gameSession.GameId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "Id", "Name", gameSession.VenueId);
            ViewData["Players"] = new MultiSelectList(_context.Players, "Id", "Name", selectedPlayers);
            return View(gameSession);
        }

        // GET: GameSessions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gameSession = await _context.GameSessions
                .Include(g => g.Game)
                .Include(g => g.SessionPlayers)
                    .ThenInclude(sp => sp.Player)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gameSession == null)
            {
                return NotFound();
            }

            return View(gameSession);
        }

        // POST: GameSessions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gameSession = await _context.GameSessions.FindAsync(id);
            if (gameSession != null)
            {
                _context.GameSessions.Remove(gameSession);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GameSessionExists(int id)
        {
            return _context.GameSessions.Any(e => e.Id == id);
        }
    }
}
