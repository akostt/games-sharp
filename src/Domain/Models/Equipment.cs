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
        public int? EquipmentTypeId { get; set; }
        
        [Display(Name = "Описание")]
        [StringLength(500, ErrorMessage = "Описание не должно превышать 500 символов")]
        public string? Description { get; set; }

        // Navigation properties
        public EquipmentType? EquipmentType { get; set; }
        public ICollection<GameEquipment> GameEquipments { get; set; } = new List<GameEquipment>();
    }
}
