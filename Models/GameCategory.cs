using System.ComponentModel.DataAnnotations;

namespace GamesSharp.Models
{
    public class GameCategory
    {
        public int Id { get; set; }
        
        [Display(Name = "Название категории")]
        [Required(ErrorMessage = "Название категории обязательно")]
        [StringLength(50, ErrorMessage = "Название не должно превышать 50 символов")]
        public string Name { get; set; } = string.Empty;
        
        [Display(Name = "Описание")]
        [StringLength(500)]
        public string? Description { get; set; }

        // Navigation property
        public ICollection<Game> Games { get; set; } = new List<Game>();
    }
}
