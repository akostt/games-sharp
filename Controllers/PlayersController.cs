using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GamesSharp.Data;
using GamesSharp.Models;
using GamesSharp.Helpers;

namespace GamesSharp.Controllers
{
    public class PlayersController : BaseController
    {
        public PlayersController(ApplicationDbContext context, ILogger<PlayersController> logger)
            : base(context, logger)
        {
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
        public async Task<IActionResult> Create([Bind("Id,Name,Email,Phone,RegisteredDate")] Player player)
        {
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Phone,RegisteredDate")] Player player)
        {
            if (id != player.Id)
            {
                return NotFoundWithLogging("Игрок", id);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Context.Players.Update(player);
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
    }
}
