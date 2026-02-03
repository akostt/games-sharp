namespace GamesSharp.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTime RegisteredDate { get; set; }

        // Navigation property
        public ICollection<SessionPlayer> SessionPlayers { get; set; } = new List<SessionPlayer>();
    }
}
