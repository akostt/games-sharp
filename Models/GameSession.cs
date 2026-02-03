namespace GamesSharp.Models
{
    public class GameSession
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public DateTime ScheduledDate { get; set; }
        public DateTime? ActualStartTime { get; set; }
        public DateTime? ActualEndTime { get; set; }
        public string? Location { get; set; }
        public string Status { get; set; } = "Planned"; // Planned, InProgress, Completed, Cancelled
        public string? Notes { get; set; }

        // Navigation properties
        public Game Game { get; set; } = null!;
        public ICollection<SessionPlayer> SessionPlayers { get; set; } = new List<SessionPlayer>();
    }
}
