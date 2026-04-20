using GamesSharp.Models;

namespace GamesSharp.Services.GameSessions
{
    public sealed record SessionValidationError(string Key, string Message);

    public sealed class SessionSelectionData
    {
        public List<Game> Games { get; init; } = new();
        public List<Venue> Venues { get; init; } = new();
        public List<Player> Players { get; init; } = new();
        public List<SessionStatus> Statuses { get; init; } = new();
    }

    public sealed class SessionSummary
    {
        public string GameName { get; init; } = string.Empty;
        public DateTime ScheduledDate { get; init; }
    }

    public sealed class EquipmentAvailabilityItem
    {
        public int EquipmentId { get; init; }
        public string EquipmentName { get; init; } = string.Empty;
        public int RequiredQuantity { get; init; }
        public int AvailableQuantity { get; init; }
        public bool IsEnough { get; init; }
    }

    public interface IGameSessionService
    {
        Task<List<GameSession>> GetSessionsForIndexAsync();
        Task<GameSession?> GetSessionDetailsAsync(int id);
        Task<GameSession?> GetSessionForEditAsync(int id);
        Task<GameSession?> GetSessionForDeleteAsync(int id);
        Task<GameSessionResultsViewModel?> GetResultsModelAsync(int id);
        Task<SessionSummary?> GetSessionSummaryAsync(int id);

        Task<SessionSelectionData> GetSelectionDataAsync();
        Task<List<SessionValidationError>> ValidateSessionInputAsync(GameSession gameSession, IEnumerable<int>? selectedPlayers);

        Task CreateSessionAsync(GameSession gameSession, IEnumerable<int>? selectedPlayers);
        Task UpdateSessionAsync(GameSession gameSession, IEnumerable<int>? selectedPlayers);
        Task<bool> SaveResultsAsync(int id, IReadOnlyCollection<SessionPlayerResultInput> submittedPlayers);
        Task<bool> DeleteSessionAsync(int id);
        Task<bool> ExistsAsync(int id);

        Task<List<EquipmentAvailabilityItem>> GetEquipmentAvailabilityAsync(int gameId, int venueId);
    }
}
