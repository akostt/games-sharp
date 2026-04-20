using System.ComponentModel.DataAnnotations;

namespace GamesSharp.Models
{
    // Many-to-many relationship between Game and GameCategory
    public class GameCategoryAssignment
    {
        public int Id { get; set; }

        public int GameId { get; set; }
        public int GameCategoryId { get; set; }

        [Display(Name = "Категория")]
        public GameCategory GameCategory { get; set; } = null!;

        [Display(Name = "Игра")]
        public Game Game { get; set; } = null!;
    }
}
