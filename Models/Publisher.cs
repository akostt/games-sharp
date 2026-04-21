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
        public int? CountryId { get; set; }
        
        [Display(Name = "Год основания")]
        public int? FoundedYear { get; set; }
        
        [Display(Name = "Веб-сайт")]
        [Url(ErrorMessage = "Некорректный URL")]
        [StringLength(200, ErrorMessage = "URL не должен превышать 200 символов")]
        public string? Website { get; set; }

        // Navigation properties
        public Country? Country { get; set; }
        public ICollection<Game> Games { get; set; } = new List<Game>();
    }
}
