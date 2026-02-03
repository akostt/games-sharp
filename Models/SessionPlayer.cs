namespace GamesSharp.Models
{
    public class SessionPlayer
    {
        public int Id { get; set; }
        public int GameSessionId { get; set; }
        public int PlayerId { get; set; }
        public int? Score { get; set; }
        public bool IsWinner { get; set; }

        // Navigation properties
        public GameSession GameSession { get; set; } = null!;
        public Player Player { get; set; } = null!;
    }
}
