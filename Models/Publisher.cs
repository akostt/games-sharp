using System.ComponentModel.DataAnnotations;

namespace GamesSharp.Models
{
    public class Publisher
    {
        public int Id { get; set; }
        
        [Display(Name = "Название издателя")]
        [Required(ErrorMessage = "Название издателя обязательно")]
        [StringLength(100, ErrorMessage = "Название не должно превышать 100 символов")]
        public string Name { get; set; } = string.Empty;
        
        [Display(Name = "Страна")]
        [StringLength(50)]
        public string? Country { get; set; }
        
        [Display(Name = "Год основания")]
        public int? FoundedYear { get; set; }
        
        [Display(Name = "Веб-сайт")]
        [Url(ErrorMessage = "Некорректный URL")]
        [StringLength(200)]
        public string? Website { get; set; }

        // Navigation property
        public ICollection<Game> Games { get; set; } = new List<Game>();
    }
}
