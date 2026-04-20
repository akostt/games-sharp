using System.ComponentModel.DataAnnotations;

namespace GamesSharp.Models
{
    // Many-to-many relationship between Venue and Equipment with available quantity.
    public class VenueEquipment
    {
        public int Id { get; set; }

        public int VenueId { get; set; }
        public int EquipmentId { get; set; }

        [Display(Name = "Количество на площадке")]
        [Range(0, 10000, ErrorMessage = "Количество должно быть от 0 до 10000")]
        public int Quantity { get; set; }

        public Venue Venue { get; set; } = null!;
        public Equipment Equipment { get; set; } = null!;
    }
}
