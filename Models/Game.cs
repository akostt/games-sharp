namespace GamesSharp.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int MinPlayers { get; set; }
        public int MaxPlayers { get; set; }
        public int AverageDuration { get; set; } // in minutes
        public string? Genre { get; set; }

        // Navigation property
        public ICollection<GameSession> GameSessions { get; set; } = new List<GameSession>();
    }
}
