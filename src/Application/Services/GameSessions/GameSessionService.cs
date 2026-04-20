using System.Globalization;
using GamesSharp.Data;
using GamesSharp.Helpers;
using GamesSharp.Models;
using Microsoft.EntityFrameworkCore;

namespace GamesSharp.Services.GameSessions
{
    public class GameSessionService : IGameSessionService
    {
        private readonly ApplicationDbContext _context;

        public GameSessionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<List<GameSession>> GetSessionsForIndexAsync()
        {
            return _context.GameSessions
                .Include(g => g.Game)
                .Include(g => g.Venue)
                .Include(g => g.SessionStatus)
                .Include(g => g.SessionPlayers)
                    .ThenInclude(sp => sp.Player)
                .AsNoTracking()
                .OrderByDescending(g => g.ScheduledDate)
                .ToListAsync();
        }

        public Task<GameSession?> GetSessionDetailsAsync(int id)
        {
            return _context.GameSessions
                .Include(g => g.Game)
                .Include(g => g.Venue)
                .Include(g => g.SessionStatus)
                .Include(g => g.SessionPlayers)
                    .ThenInclude(sp => sp.Player)
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public Task<GameSession?> GetSessionForEditAsync(int id)
        {
            return _context.GameSessions
                .Include(g => g.SessionPlayers)
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public Task<GameSession?> GetSessionForDeleteAsync(int id)
        {
            return _context.GameSessions
                .Include(g => g.Game)
                .Include(g => g.SessionStatus)
                .Include(g => g.SessionPlayers)
                    .ThenInclude(sp => sp.Player)
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<GameSessionResultsViewModel?> GetResultsModelAsync(int id)
        {
            var gameSession = await _context.GameSessions
                .Include(gs => gs.Game)
                .Include(gs => gs.SessionPlayers)
                    .ThenInclude(sp => sp.Player)
                .AsNoTracking()
                .FirstOrDefaultAsync(gs => gs.Id == id);

            if (gameSession == null)
            {
                return null;
            }

            return new GameSessionResultsViewModel
            {
                GameSessionId = gameSession.Id,
                GameName = gameSession.Game?.Name ?? "Без названия",
                ScheduledDate = gameSession.ScheduledDate,
                Players = gameSession.SessionPlayers
                    .OrderBy(sp => sp.Player != null ? sp.Player.Name : string.Empty)
                    .Select(sp => new SessionPlayerResultInput
                    {
                        SessionPlayerId = sp.Id,
                        PlayerId = sp.PlayerId,
                        PlayerName = sp.Player?.Name ?? "Игрок",
                        Score = sp.Score,
                        IsWinner = sp.IsWinner
                    })
                    .ToList()
            };
        }

        public async Task<SessionSummary?> GetSessionSummaryAsync(int id)
        {
            var session = await _context.GameSessions
                .Include(gs => gs.Game)
                .AsNoTracking()
                .FirstOrDefaultAsync(gs => gs.Id == id);

            if (session == null)
            {
                return null;
            }

            return new SessionSummary
            {
                GameName = session.Game?.Name ?? "Без названия",
                ScheduledDate = session.ScheduledDate
            };
        }

        public async Task<SessionSelectionData> GetSelectionDataAsync()
        {
            var games = await _context.Games
                .AsNoTracking()
                .OrderBy(g => g.Name)
                .ToListAsync();

            var venues = await _context.Venues
                .AsNoTracking()
                .OrderBy(v => v.Name)
                .ToListAsync();

            var players = await _context.Players
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .ToListAsync();

            var statuses = await _context.SessionStatuses
                .AsNoTracking()
                .OrderBy(s => s.Id)
                .ToListAsync();

            return new SessionSelectionData
            {
                Games = games,
                Venues = venues,
                Players = players,
                Statuses = statuses
            };
        }

        public async Task<List<SessionValidationError>> ValidateSessionInputAsync(GameSession gameSession, IEnumerable<int>? selectedPlayers)
        {
            var errors = new List<SessionValidationError>();

            if (gameSession.ActualStartTime.HasValue && gameSession.ActualEndTime.HasValue &&
                gameSession.ActualEndTime.Value < gameSession.ActualStartTime.Value)
            {
                errors.Add(new SessionValidationError(
                    nameof(GameSession.ActualEndTime),
                    "Время завершения не может быть раньше времени начала"));
            }

            if (gameSession.MaxParticipants.HasValue && gameSession.MaxParticipants <= 0)
            {
                errors.Add(new SessionValidationError(
                    nameof(GameSession.MaxParticipants),
                    "Максимальное количество участников должно быть больше нуля"));
            }

            var selectedCount = selectedPlayers?.Distinct().Count() ?? 0;
            if (gameSession.MaxParticipants.HasValue && selectedCount > gameSession.MaxParticipants.Value)
            {
                errors.Add(new SessionValidationError(
                    nameof(GameSession.MaxParticipants),
                    "Количество выбранных участников не может превышать ограничение сессии"));
            }

            if (gameSession.VenueId.HasValue)
            {
                errors.AddRange(await ValidateVenueEquipmentAvailabilityAsync(gameSession.GameId, gameSession.VenueId.Value));
            }

            return errors;
        }

        public async Task CreateSessionAsync(GameSession gameSession, IEnumerable<int>? selectedPlayers)
        {
            NormalizeSessionTimes(gameSession);
            gameSession.SessionStatusId = Constants.SessionStatus.ScheduledId;

            _context.GameSessions.Add(gameSession);
            await _context.SaveChangesAsync();

            await ReplaceSessionPlayersAsync(gameSession.Id, selectedPlayers);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateSessionAsync(GameSession gameSession, IEnumerable<int>? selectedPlayers)
        {
            NormalizeSessionTimes(gameSession);
            _context.GameSessions.Update(gameSession);

            await ReplaceSessionPlayersAsync(gameSession.Id, selectedPlayers);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> SaveResultsAsync(int id, IReadOnlyCollection<SessionPlayerResultInput> submittedPlayers)
        {
            var gameSession = await _context.GameSessions
                .Include(gs => gs.SessionPlayers)
                .FirstOrDefaultAsync(gs => gs.Id == id);

            if (gameSession == null)
            {
                return false;
            }

            var playersById = gameSession.SessionPlayers.ToDictionary(sp => sp.Id);
            foreach (var input in submittedPlayers)
            {
                if (!playersById.TryGetValue(input.SessionPlayerId, out var sessionPlayer))
                {
                    continue;
                }

                sessionPlayer.Score = input.Score;
                sessionPlayer.IsWinner = input.IsWinner;
            }

            if (submittedPlayers.Any(p => p.Score.HasValue || p.IsWinner))
            {
                gameSession.SessionStatusId = Constants.SessionStatus.CompletedId;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteSessionAsync(int id)
        {
            var gameSession = await _context.GameSessions.FindAsync(id);
            if (gameSession == null)
            {
                return false;
            }

            _context.GameSessions.Remove(gameSession);
            await _context.SaveChangesAsync();
            return true;
        }

        public Task<bool> ExistsAsync(int id)
        {
            return _context.GameSessions.AnyAsync(e => e.Id == id);
        }

        public async Task<List<EquipmentAvailabilityItem>> GetEquipmentAvailabilityAsync(int gameId, int venueId)
        {
            var required = await _context.GameEquipments
                .AsNoTracking()
                .Where(ge => ge.GameId == gameId)
                .Select(ge => new
                {
                    ge.EquipmentId,
                    EquipmentName = ge.Equipment.Name,
                    RequiredQuantity = ge.RequiredQuantity
                })
                .ToListAsync();

            var venueStock = await _context.VenueEquipments
                .AsNoTracking()
                .Where(ve => ve.VenueId == venueId)
                .ToDictionaryAsync(ve => ve.EquipmentId, ve => ve.Quantity);

            return required
                .Select(item => new EquipmentAvailabilityItem
                {
                    EquipmentId = item.EquipmentId,
                    EquipmentName = item.EquipmentName,
                    RequiredQuantity = item.RequiredQuantity,
                    AvailableQuantity = venueStock.TryGetValue(item.EquipmentId, out var quantity) ? quantity : 0,
                    IsEnough = venueStock.TryGetValue(item.EquipmentId, out var available) && available >= item.RequiredQuantity
                })
                .OrderBy(x => x.EquipmentName)
                .ToList();
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
            var existingPlayers = await _context.SessionPlayers
                .Where(sp => sp.GameSessionId == gameSessionId)
                .ToListAsync();

            var uniquePlayers = (selectedPlayers ?? Enumerable.Empty<int>()).Distinct().ToHashSet();
            var toRemove = existingPlayers.Where(sp => !uniquePlayers.Contains(sp.PlayerId)).ToList();

            if (toRemove.Count > 0)
            {
                _context.SessionPlayers.RemoveRange(toRemove);
            }

            var existingPlayerIds = existingPlayers.Select(sp => sp.PlayerId).ToHashSet();
            foreach (var playerId in uniquePlayers)
            {
                if (existingPlayerIds.Contains(playerId))
                {
                    continue;
                }

                _context.SessionPlayers.Add(new SessionPlayer
                {
                    GameSessionId = gameSessionId,
                    PlayerId = playerId
                });
            }
        }

        private async Task<List<SessionValidationError>> ValidateVenueEquipmentAvailabilityAsync(int gameId, int venueId)
        {
            var required = await _context.GameEquipments
                .AsNoTracking()
                .Where(ge => ge.GameId == gameId)
                .Select(ge => new
                {
                    ge.EquipmentId,
                    EquipmentName = ge.Equipment.Name,
                    ge.RequiredQuantity
                })
                .ToListAsync();

            if (!required.Any())
            {
                return new List<SessionValidationError>();
            }

            var stock = await _context.VenueEquipments
                .AsNoTracking()
                .Where(ve => ve.VenueId == venueId)
                .ToDictionaryAsync(ve => ve.EquipmentId, ve => ve.Quantity);

            var errors = new List<SessionValidationError>();
            foreach (var item in required)
            {
                var available = stock.TryGetValue(item.EquipmentId, out var quantity) ? quantity : 0;
                if (available < item.RequiredQuantity)
                {
                    var shortage = item.RequiredQuantity - available;
                    errors.Add(new SessionValidationError(
                        nameof(GameSession.VenueId),
                        string.Format(
                            CultureInfo.CurrentCulture,
                            "На выбранной площадке недостаточно инвентаря \"{0}\": нужно {1}, доступно {2} (не хватает {3}).",
                            item.EquipmentName,
                            item.RequiredQuantity,
                            available,
                            shortage)));
                }
            }

            return errors;
        }
    }
}
