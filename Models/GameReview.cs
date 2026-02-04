using System.ComponentModel.DataAnnotations;

namespace GamesSharp.Models
{
    public class GameReview
    {
        public int Id { get; set; }
        
        public int GameId { get; set; }
        public int PlayerId { get; set; }
        
        [Display(Name = "Рейтинг")]
        [Required(ErrorMessage = "Рейтинг обязателен")]
        [Range(1, 10, ErrorMessage = "Рейтинг должен быть от 1 до 10")]
        public int Rating { get; set; }
        
        [Display(Name = "Отзыв")]
        [StringLength(1000, ErrorMessage = "Отзыв не должен превышать 1000 символов")]
        public string? ReviewText { get; set; }
        
        [Display(Name = "Дата отзыва")]
        public DateTime ReviewDate { get; set; }

        // Navigation properties
        public Game Game { get; set; } = null!;
        public Player Player { get; set; } = null!;
    }
}
