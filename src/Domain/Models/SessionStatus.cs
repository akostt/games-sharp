using System.ComponentModel.DataAnnotations;

namespace GamesSharp.Models
{
    public class SessionStatus
    {
        public int Id { get; set; }

        [Display(Name = "Код")]
        [Required(ErrorMessage = "Код обязателен")]
        [StringLength(30, ErrorMessage = "Код не должен превышать 30 символов")]
        public string Code { get; set; } = string.Empty;

        [Display(Name = "Название")]
        [Required(ErrorMessage = "Название обязательно")]
        [StringLength(50, ErrorMessage = "Название не должно превышать 50 символов")]
        public string Name { get; set; } = string.Empty;

        public ICollection<GameSession> GameSessions { get; set; } = new List<GameSession>();
    }
}
