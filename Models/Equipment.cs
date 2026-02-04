using System.ComponentModel.DataAnnotations;

namespace GamesSharp.Models
{
    public class Equipment
    {
        public int Id { get; set; }
        
        [Display(Name = "Название оборудования")]
        [Required(ErrorMessage = "Название оборудования обязательно")]
        [StringLength(100, ErrorMessage = "Название не должно превышать 100 символов")]
        public string Name { get; set; } = string.Empty;
        
        [Display(Name = "Тип")]
        [StringLength(50)]
        public string? Type { get; set; } // Dice, Cards, Timer, Mat, etc.
        
        [Display(Name = "Количество")]
        [Range(0, 10000, ErrorMessage = "Количество должно быть от 0 до 10000")]
        public int Quantity { get; set; }
        
        [Display(Name = "Описание")]
        [StringLength(500)]
        public string? Description { get; set; }

        // Navigation properties
        public ICollection<GameEquipment> GameEquipments { get; set; } = new List<GameEquipment>();
    }
}
