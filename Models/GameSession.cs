using System.ComponentModel.DataAnnotations;

namespace GamesSharp.Models
{
    public class GameSession
    {
        public int Id { get; set; }
        
        [Display(Name = "Игра")]
        [Required(ErrorMessage = "Выберите игру")]
        public int GameId { get; set; }
        
        [Display(Name = "Место проведения")]
        public int? VenueId { get; set; }
        
        [Display(Name = "Запланированная дата")]
        [Required(ErrorMessage = "Запланированная дата обязательна")]
        public DateTime ScheduledDate { get; set; }
        
        [Display(Name = "Время начала")]
        public DateTime? ActualStartTime { get; set; }
        
        [Display(Name = "Время окончания")]
        public DateTime? ActualEndTime { get; set; }
        
        [Display(Name = "Статус")]
        [Required(ErrorMessage = "Статус обязателен")]
        public int SessionStatusId { get; set; } = 1;
        
        [Display(Name = "Заметки")]
        [StringLength(500, ErrorMessage = "Заметки не должны превышать 500 символов")]
        public string? Notes { get; set; }
        
        [Display(Name = "Организатор")]
        [StringLength(100, ErrorMessage = "Имя организатора не должно превышать 100 символов")]
        public string? Organizer { get; set; }
        
        [Display(Name = "Макс. участников")]
        [Range(1, 100, ErrorMessage = "Максимальное количество участников должно быть от 1 до 100")]
        public int? MaxParticipants { get; set; }

        // Navigation properties
        public Game? Game { get; set; }
        public Venue? Venue { get; set; }
        public SessionStatus? SessionStatus { get; set; }
        public ICollection<SessionPlayer> SessionPlayers { get; set; } = new List<SessionPlayer>();
    }
}
