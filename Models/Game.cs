using System.ComponentModel.DataAnnotations;
using GamesSharp.Validation;

namespace GamesSharp.Models
{
    public class Game
    {
        public int Id { get; set; }
        
        [Display(Name = "Название игры")]
        [Required(ErrorMessage = "Название игры обязательно")]
        [StringLength(100, ErrorMessage = "Название не должно превышать 100 символов")]
        public string Name { get; set; } = string.Empty;
        
        [Display(Name = "Описание")]
        [StringLength(1000, ErrorMessage = "Описание не должно превышать 1000 символов")]
        public string? Description { get; set; }
        
        [Display(Name = "Мин. игроков")]
        [Required(ErrorMessage = "Минимальное количество игроков обязательно")]
        [Range(1, 100, ErrorMessage = "Количество игроков должно быть от 1 до 100")]
        public int MinPlayers { get; set; }
        
        [Display(Name = "Макс. игроков")]
        [Required(ErrorMessage = "Максимальное количество игроков обязательно")]
        [Range(1, 100, ErrorMessage = "Количество игроков должно быть от 1 до 100")]
        [GreaterThanOrEqual(nameof(MinPlayers), ErrorMessage = "Максимальное количество игроков должно быть больше или равно минимальному")]
        public int MaxPlayers { get; set; }
        
        [Display(Name = "Средняя длительность (мин)")]
        [Range(1, 1000, ErrorMessage = "Длительность должна быть от 1 до 1000 минут")]
        public int AverageDuration { get; set; }
        
        [Display(Name = "Сложность")]
        [Range(1, 10, ErrorMessage = "Сложность должна быть от 1 до 10")]
        public int? Complexity { get; set; }
        
        [Display(Name = "Минимальный возраст")]
        [Range(0, 99, ErrorMessage = "Возраст должен быть от 0 до 99")]
        public int? MinAge { get; set; }
        
        [Display(Name = "Год издания")]
        [ValidYear(ErrorMessage = "Год издания должен быть корректным")]
        public int? YearPublished { get; set; }
        
        // Foreign keys
        public int? PublisherId { get; set; }

        // Navigation properties
        public Publisher? Publisher { get; set; }
        public ICollection<GameSession> GameSessions { get; set; } = new List<GameSession>();
        public ICollection<GameEquipment> GameEquipments { get; set; } = new List<GameEquipment>();
        public ICollection<GameCategoryAssignment> GameCategoryAssignments { get; set; } = new List<GameCategoryAssignment>();
    }
}
