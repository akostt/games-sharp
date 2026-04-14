using System.ComponentModel.DataAnnotations;

namespace GamesSharp.Models
{
    public class Achievement
    {
        public int Id { get; set; }
        
        public int PlayerId { get; set; }
        
        [Display(Name = "Название достижения")]
        [Required(ErrorMessage = "Название достижения обязательно")]
        [StringLength(100, ErrorMessage = "Название не должно превышать 100 символов")]
        public string Name { get; set; } = string.Empty;
        
        [Display(Name = "Описание")]
        [StringLength(500, ErrorMessage = "Описание не должно превышать 500 символов")]
        public string? Description { get; set; }
        
        [Display(Name = "Дата получения")]
        public DateTime AchievedDate { get; set; }
        
        [Display(Name = "Категория")]
        [StringLength(50, ErrorMessage = "Название категории не должно превышать 50 символов")]
        public string? Category { get; set; } // Victory, Participation, Special, etc.

        // Navigation property
        public Player Player { get; set; } = null!;
    }
}
