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
        public DbSet<GameCategory> GameCategories { get; set; }
        public DbSet<GameCategoryAssignment> GameCategoryAssignments { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<Equipment> Equipments { get; set; }
        public DbSet<GameEquipment> GameEquipments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Game relationships
            modelBuilder.Entity<Game>()
                .HasOne(g => g.Publisher)
                .WithMany(p => p.Games)
                .HasForeignKey(g => g.PublisherId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure GameCategoryAssignment relationships (Many-to-Many)
            modelBuilder.Entity<GameCategoryAssignment>()
                .HasOne(gca => gca.Game)
                .WithMany(g => g.GameCategoryAssignments)
                .HasForeignKey(gca => gca.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GameCategoryAssignment>()
                .HasOne(gca => gca.GameCategory)
                .WithMany(gc => gc.GameCategoryAssignments)
                .HasForeignKey(gca => gca.GameCategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure GameSession relationships
            modelBuilder.Entity<GameSession>()
                .HasOne(gs => gs.Game)
                .WithMany(g => g.GameSessions)
                .HasForeignKey(gs => gs.GameId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GameSession>()
                .HasOne(gs => gs.Venue)
                .WithMany(v => v.GameSessions)
                .HasForeignKey(gs => gs.VenueId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure SessionPlayer relationships
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

            // Configure GameEquipment relationships (Many-to-Many)
            modelBuilder.Entity<GameEquipment>()
                .HasOne(ge => ge.Game)
                .WithMany(g => g.GameEquipments)
                .HasForeignKey(ge => ge.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GameEquipment>()
                .HasOne(ge => ge.Equipment)
                .WithMany(e => e.GameEquipments)
                .HasForeignKey(ge => ge.EquipmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed initial data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed GameCategories
            modelBuilder.Entity<GameCategory>().HasData(
                new GameCategory { Id = 1, Name = "Стратегия", Description = "Игры, требующие стратегического мышления" },
                new GameCategory { Id = 2, Name = "Семейная", Description = "Игры для всей семьи" },
                new GameCategory { Id = 3, Name = "Кооператив", Description = "Совместные игры" },
                new GameCategory { Id = 4, Name = "Карточная", Description = "Карточные игры" },
                new GameCategory { Id = 5, Name = "Партийная", Description = "Легкие игры для компании" }
            );

            // Seed Publishers
            modelBuilder.Entity<Publisher>().HasData(
                new Publisher { Id = 1, Name = "Hobby World", Country = "Россия", FoundedYear = 2009, Website = "https://www.hobbyworld.ru" },
                new Publisher { Id = 2, Name = "Cosmodrome Games", Country = "Россия", FoundedYear = 2011, Website = "https://cosmodrome.games" },
                new Publisher { Id = 3, Name = "Days of Wonder", Country = "США", FoundedYear = 2002, Website = "https://www.daysofwonder.com" }
            );

            // Seed Venues
            modelBuilder.Entity<Venue>().HasData(
                new Venue { Id = 1, Name = "Игротека на Арбате", Address = "ул. Арбат, д. 10", Capacity = 30, Phone = "+7(495)123-45-67", RentalCostPerHour = 500 },
                new Venue { Id = 2, Name = "Антикафе Таймкод", Address = "Ленинский пр-т, д. 5", Capacity = 20, Phone = "+7(495)234-56-78", RentalCostPerHour = 300 },
                new Venue { Id = 3, Name = "Клуб настольных игр Мосигра", Address = "пр-т Мира, д. 33", Capacity = 50, Phone = "+7(495)345-67-89", RentalCostPerHour = 800 }
            );

            // Seed Equipment
            modelBuilder.Entity<Equipment>().HasData(
                new Equipment { Id = 1, Name = "Игральные кости (6-гранные)", Type = "Кости", Quantity = 100 },
                new Equipment { Id = 2, Name = "Игровой таймер", Type = "Таймер", Quantity = 10 },
                new Equipment { Id = 3, Name = "Игровой коврик", Type = "Аксессуар", Quantity = 5 },
                new Equipment { Id = 4, Name = "Фишки (набор)", Type = "Фишки", Quantity = 50 }
            );

            // Seed Games (популярные в России)
            modelBuilder.Entity<Game>().HasData(
                new Game 
                { 
                    Id = 1, 
                    Name = "Манчкин", 
                    Description = "Пародийная карточная игра на тему фэнтези-подземелий. Убивай монстров, предавай друзей, хватай сокровища!", 
                    MinPlayers = 3, 
                    MaxPlayers = 6, 
                    AverageDuration = 60, 
                    PublisherId = 1,
                    Complexity = 3,
                    MinAge = 10,
                    YearPublished = 2001
                },
                new Game 
                { 
                    Id = 2, 
                    Name = "Имаджинариум", 
                    Description = "Российская игра на ассоциации с красивыми иллюстрациями. Придумывай ассоциации к картинкам!", 
                    MinPlayers = 4, 
                    MaxPlayers = 7, 
                    AverageDuration = 45, 
                    PublisherId = 1,
                    Complexity = 2,
                    MinAge = 12,
                    YearPublished = 2011
                },
                new Game 
                { 
                    Id = 3, 
                    Name = "Взрывные котята", 
                    Description = "Весёлая карточная игра в русскую рулетку с котятами, лазерами и козами", 
                    MinPlayers = 2, 
                    MaxPlayers = 5, 
                    AverageDuration = 15, 
                    PublisherId = 1,
                    Complexity = 1,
                    MinAge = 7,
                    YearPublished = 2015
                },
                new Game 
                { 
                    Id = 4, 
                    Name = "Кодовые имена", 
                    Description = "Командная игра на ассоциации и дедукцию. Угадывай слова по намёкам капитана!", 
                    MinPlayers = 4, 
                    MaxPlayers = 8, 
                    AverageDuration = 15, 
                    PublisherId = 2,
                    Complexity = 2,
                    MinAge = 10,
                    YearPublished = 2015
                },
                new Game 
                { 
                    Id = 5, 
                    Name = "Колонизаторы", 
                    Description = "Классическая стратегия о колонизации острова Катан. Торгуй, строй, побеждай!", 
                    MinPlayers = 3, 
                    MaxPlayers = 4, 
                    AverageDuration = 75, 
                    PublisherId = 1,
                    Complexity = 6,
                    MinAge = 10,
                    YearPublished = 1995
                },
                new Game 
                { 
                    Id = 6, 
                    Name = "Элиас", 
                    Description = "Популярная игра в объяснение слов. Объясняй слова, не используя однокоренные!", 
                    MinPlayers = 4, 
                    MaxPlayers = 12, 
                    AverageDuration = 45, 
                    PublisherId = 2,
                    Complexity = 1,
                    MinAge = 7,
                    YearPublished = 1993
                },
                new Game 
                { 
                    Id = 7, 
                    Name = "Uno", 
                    Description = "Знаменитая карточная игра. Избавься от всех карт первым!", 
                    MinPlayers = 2, 
                    MaxPlayers = 10, 
                    AverageDuration = 30, 
                    PublisherId = 2,
                    Complexity = 1,
                    MinAge = 7,
                    YearPublished = 1971
                },
                new Game 
                { 
                    Id = 8, 
                    Name = "Каркассон", 
                    Description = "Игра на выкладывание тайлов средневекового французского ландшафта", 
                    MinPlayers = 2, 
                    MaxPlayers = 5, 
                    AverageDuration = 40, 
                    PublisherId = 1,
                    Complexity = 4,
                    MinAge = 8,
                    YearPublished = 2000
                }
            );

            modelBuilder.Entity<GameCategoryAssignment>().HasData(
                new GameCategoryAssignment { Id = 1, GameId = 1, GameCategoryId = 4 },
                new GameCategoryAssignment { Id = 2, GameId = 2, GameCategoryId = 5 },
                new GameCategoryAssignment { Id = 3, GameId = 3, GameCategoryId = 4 },
                new GameCategoryAssignment { Id = 4, GameId = 4, GameCategoryId = 5 },
                new GameCategoryAssignment { Id = 5, GameId = 5, GameCategoryId = 1 },
                new GameCategoryAssignment { Id = 6, GameId = 6, GameCategoryId = 5 },
                new GameCategoryAssignment { Id = 7, GameId = 7, GameCategoryId = 4 },
                new GameCategoryAssignment { Id = 8, GameId = 8, GameCategoryId = 1 }
            );
        }
    }
}
