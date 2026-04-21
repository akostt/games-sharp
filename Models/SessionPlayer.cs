using System.ComponentModel.DataAnnotations;

namespace GamesSharp.Models
{
    public class SessionPlayer
    {
        public int Id { get; set; }
        
        public int GameSessionId { get; set; }
        public int PlayerId { get; set; }
        
        [Display(Name = "Очки")]
        [Range(0, 1000000, ErrorMessage = "Очки должны быть от 0 до 1000000")]
        public int? Score { get; set; }
        
        [Display(Name = "Победитель")]
        public bool IsWinner { get; set; }

        // Navigation properties
        public GameSession GameSession { get; set; } = null!;
        public Player Player { get; set; } = null!;
    }
}
