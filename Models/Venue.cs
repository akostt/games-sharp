using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

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
        [StringLength(200, ErrorMessage = "Адрес не должен превышать 200 символов")]
        public string Address { get; set; } = string.Empty;
        
        [Display(Name = "Вместимость")]
        [Required(ErrorMessage = "Вместимость обязательна")]
        [Range(1, 1000, ErrorMessage = "Вместимость должна быть от 1 до 1000")]
        public int Capacity { get; set; }
        
        [Display(Name = "Телефон")]
        [Phone(ErrorMessage = "Некорректный номер телефона")]
        [StringLength(20, ErrorMessage = "Телефон не должен превышать 20 символов")]
        public string? Phone { get; set; }
        
        [Display(Name = "Стоимость аренды (руб./час)")]
        [Range(0, 100000, ErrorMessage = "Стоимость должна быть от 0 до 100000")]
        [Precision(10, 2)]
        public decimal? RentalCostPerHour { get; set; }
        
        [Display(Name = "Описание")]
        [StringLength(500, ErrorMessage = "Описание не должно превышать 500 символов")]
        public string? Description { get; set; }

        [Display(Name = "Широта")]
        [Range(-90, 90, ErrorMessage = "Широта должна быть в диапазоне от -90 до 90")]
        public double? Latitude { get; set; }

        [Display(Name = "Долгота")]
        [Range(-180, 180, ErrorMessage = "Долгота должна быть в диапазоне от -180 до 180")]
        public double? Longitude { get; set; }

        // Navigation property
        public ICollection<GameSession> GameSessions { get; set; } = new List<GameSession>();
        public ICollection<VenueEquipment> VenueEquipments { get; set; } = new List<VenueEquipment>();
    }
}
