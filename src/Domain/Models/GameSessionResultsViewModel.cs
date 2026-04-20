using System.ComponentModel.DataAnnotations;

namespace GamesSharp.Models
{
    public class GameSessionResultsViewModel
    {
        public int GameSessionId { get; set; }

        public string GameName { get; set; } = string.Empty;

        public DateTime ScheduledDate { get; set; }

        public List<SessionPlayerResultInput> Players { get; set; } = new();
    }

    public class SessionPlayerResultInput
    {
        public int SessionPlayerId { get; set; }

        public int PlayerId { get; set; }

        public string PlayerName { get; set; } = string.Empty;

        [Display(Name = "Очки")]
        [Range(0, 1000000, ErrorMessage = "Очки должны быть от 0 до 1000000")]
        public int? Score { get; set; }

        [Display(Name = "Победитель")]
        public bool IsWinner { get; set; }
    }
}
