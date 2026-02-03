using Microsoft.EntityFrameworkCore;
using GamesSharp.Models;

namespace GamesSharp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Game> Games { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<GameSession> GameSessions { get; set; }
        public DbSet<SessionPlayer> SessionPlayers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<GameSession>()
                .HasOne(gs => gs.Game)
                .WithMany(g => g.GameSessions)
                .HasForeignKey(gs => gs.GameId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SessionPlayer>()
                .HasOne(sp => sp.GameSession)
                .WithMany(gs => gs.SessionPlayers)
                .HasForeignKey(sp => sp.GameSessionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SessionPlayer>()
                .HasOne(sp => sp.Player)
                .WithMany(p => p.SessionPlayers)
                .HasForeignKey(sp => sp.PlayerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed initial data
            modelBuilder.Entity<Game>().HasData(
                new Game { Id = 1, Name = "Catan", Description = "Strategy board game", MinPlayers = 3, MaxPlayers = 4, AverageDuration = 90, Genre = "Strategy" },
                new Game { Id = 2, Name = "Ticket to Ride", Description = "Railway-themed board game", MinPlayers = 2, MaxPlayers = 5, AverageDuration = 60, Genre = "Family" },
                new Game { Id = 3, Name = "Pandemic", Description = "Cooperative board game", MinPlayers = 2, MaxPlayers = 4, AverageDuration = 45, Genre = "Cooperative" }
            );
        }
    }
}
