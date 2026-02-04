using System.ComponentModel.DataAnnotations;

namespace GamesSharp.Models
{
    public class Venue
    {
        public int Id { get; set; }
        
        [Display(Name = "Название места")]
        [Required(ErrorMessage = "Название места обязательно")]
        [StringLength(100, ErrorMessage = "Название не должно превышать 100 символов")]
        public string Name { get; set; } = string.Empty;
        
        [Display(Name = "Адрес")]
        [Required(ErrorMessage = "Адрес обязателен")]
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;
        
        [Display(Name = "Вместимость")]
        [Range(1, 1000, ErrorMessage = "Вместимость должна быть от 1 до 1000")]
        public int Capacity { get; set; }
        
        [Display(Name = "Телефон")]
        [Phone(ErrorMessage = "Некорректный номер телефона")]
        [StringLength(20)]
        public string? Phone { get; set; }
        
        [Display(Name = "Стоимость аренды (руб./час)")]
        [Range(0, 100000, ErrorMessage = "Стоимость должна быть от 0 до 100000")]
        public decimal? RentalCostPerHour { get; set; }
        
        [Display(Name = "Описание")]
        [StringLength(500)]
        public string? Description { get; set; }

        // Navigation property
        public ICollection<GameSession> GameSessions { get; set; } = new List<GameSession>();
    }
}
