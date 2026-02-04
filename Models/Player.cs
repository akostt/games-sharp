using System.ComponentModel.DataAnnotations;

namespace GamesSharp.Models
{
    public class Player
    {
        public int Id { get; set; }
        
        [Display(Name = "Имя игрока")]
        [Required(ErrorMessage = "Имя игрока обязательно")]
        [StringLength(100, ErrorMessage = "Имя не должно превышать 100 символов")]
        public string Name { get; set; } = string.Empty;
        
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Некорректный формат email (example@domain.com)")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Email должен быть в формате: example@domain.com")]
        [StringLength(100)]
        public string? Email { get; set; }
        
        [Display(Name = "Телефон")]
        [RegularExpression(@"^\+7\s?\(\d{3}\)\s?\d{3}-\d{2}-\d{2}$", ErrorMessage = "Телефон должен быть в формате: +7 (XXX) XXX-XX-XX")]
        [StringLength(20)]
        public string? Phone { get; set; }
        
        [Display(Name = "Дата регистрации")]
        public DateTime RegisteredDate { get; set; }
        
        [Display(Name = "Дата рождения")]
        public DateTime? BirthDate { get; set; }
        
        [Display(Name = "Город")]
        [StringLength(50)]
        public string? City { get; set; }
        
        [Display(Name = "Любимый жанр")]
        [StringLength(50)]
        public string? FavoriteGenre { get; set; }

        // Navigation properties
        public ICollection<SessionPlayer> SessionPlayers { get; set; } = new List<SessionPlayer>();
        public ICollection<GameReview> GameReviews { get; set; } = new List<GameReview>();
        public ICollection<Achievement> Achievements { get; set; } = new List<Achievement>();
    }
}
