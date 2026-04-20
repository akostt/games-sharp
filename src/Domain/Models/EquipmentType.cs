using System.ComponentModel.DataAnnotations;

namespace GamesSharp.Models
{
    public class EquipmentType
    {
        public int Id { get; set; }

        [Display(Name = "Тип оборудования")]
        [Required(ErrorMessage = "Название типа обязательно")]
        [StringLength(50, ErrorMessage = "Название типа не должно превышать 50 символов")]
        public string Name { get; set; } = string.Empty;

        public ICollection<Equipment> Equipments { get; set; } = new List<Equipment>();
    }
}
