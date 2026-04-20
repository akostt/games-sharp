using System.ComponentModel.DataAnnotations;

namespace GamesSharp.Models
{
    // Many-to-many relationship between Game and Equipment
    public class GameEquipment
    {
        public int Id { get; set; }
        
        public int GameId { get; set; }
        public int EquipmentId { get; set; }
        
        [Display(Name = "Требуемое количество")]
        [Range(1, 1000, ErrorMessage = "Количество должно быть от 1 до 1000")]
        public int RequiredQuantity { get; set; }

        // Navigation properties
        public Game Game { get; set; } = null!;
        public Equipment Equipment { get; set; } = null!;
    }
}
