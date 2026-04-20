using System.ComponentModel.DataAnnotations;

namespace GamesSharp.Models
{
    public class Country
    {
        public int Id { get; set; }

        [Display(Name = "Страна")]
        [Required(ErrorMessage = "Название страны обязательно")]
        [StringLength(50, ErrorMessage = "Название страны не должно превышать 50 символов")]
        public string Name { get; set; } = string.Empty;

        public ICollection<Publisher> Publishers { get; set; } = new List<Publisher>();
    }
}
