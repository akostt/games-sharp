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
        public DbSet<Country> Countries { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<EquipmentType> EquipmentTypes { get; set; }
        public DbSet<Equipment> Equipments { get; set; }
        public DbSet<GameEquipment> GameEquipments { get; set; }
        public DbSet<SessionStatus> SessionStatuses { get; set; }
        public DbSet<VenueEquipment> VenueEquipments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Game relationships
            modelBuilder.Entity<Game>()
                .HasOne(g => g.Publisher)
                .WithMany(p => p.Games)
                .HasForeignKey(g => g.PublisherId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Country>()
                .HasIndex(c => c.Name)
                .IsUnique();

            modelBuilder.Entity<EquipmentType>()
                .HasIndex(et => et.Name)
                .IsUnique();

            modelBuilder.Entity<SessionStatus>()
                .HasIndex(ss => ss.Code)
                .IsUnique();

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

            modelBuilder.Entity<GameSession>()
                .HasOne(gs => gs.SessionStatus)
                .WithMany(ss => ss.GameSessions)
                .HasForeignKey(gs => gs.SessionStatusId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Publisher>()
                .HasOne(p => p.Country)
                .WithMany(c => c.Publishers)
                .HasForeignKey(p => p.CountryId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Equipment>()
                .HasOne(e => e.EquipmentType)
                .WithMany(et => et.Equipments)
                .HasForeignKey(e => e.EquipmentTypeId)
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

            // Configure VenueEquipment relationships (Many-to-Many with quantity)
            modelBuilder.Entity<VenueEquipment>()
                .HasOne(ve => ve.Venue)
                .WithMany(v => v.VenueEquipments)
                .HasForeignKey(ve => ve.VenueId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<VenueEquipment>()
                .HasOne(ve => ve.Equipment)
                .WithMany()
                .HasForeignKey(ve => ve.EquipmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VenueEquipment>()
                .HasIndex(ve => new { ve.VenueId, ve.EquipmentId })
                .IsUnique();

            // Seed initial data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Country>().HasData(
                new Country { Id = 1, Name = "Россия" },
                new Country { Id = 2, Name = "США" }
            );

            modelBuilder.Entity<EquipmentType>().HasData(
                new EquipmentType { Id = 1, Name = "Кости" },
                new EquipmentType { Id = 2, Name = "Таймер" },
                new EquipmentType { Id = 3, Name = "Аксессуар" },
                new EquipmentType { Id = 4, Name = "Фишки" }
            );

            modelBuilder.Entity<SessionStatus>().HasData(
                new SessionStatus { Id = 1, Code = "scheduled", Name = "Запланирована" },
                new SessionStatus { Id = 2, Code = "in_progress", Name = "В процессе" },
                new SessionStatus { Id = 3, Code = "completed", Name = "Завершена" },
                new SessionStatus { Id = 4, Code = "cancelled", Name = "Отменена" }
            );

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
                new Publisher { Id = 1, Name = "Hobby World", CountryId = 1, FoundedYear = 2009, Website = "https://www.hobbyworld.ru" },
                new Publisher { Id = 2, Name = "Cosmodrome Games", CountryId = 1, FoundedYear = 2011, Website = "https://cosmodrome.games" },
                new Publisher { Id = 3, Name = "Days of Wonder", CountryId = 2, FoundedYear = 2002, Website = "https://www.daysofwonder.com" }
            );

            // Seed Venues
            modelBuilder.Entity<Venue>().HasData(
                new Venue { Id = 1, Name = "Клуб настольных игр Владимир", Address = "г. Владимир, ул. Большая Московская, д. 11", Capacity = 40, Phone = "+7(492)212-34-56", RentalCostPerHour = 600, Latitude = 56.1291, Longitude = 40.4066 },
                new Venue { Id = 2, Name = "Игротека Суздаль", Address = "г. Суздаль, ул. Ленина, д. 72", Capacity = 24, Phone = "+7(492)312-45-67", RentalCostPerHour = 450, Latitude = 56.4195, Longitude = 40.4525 },
                new Venue { Id = 3, Name = "Антикафе Муром", Address = "г. Муром, ул. Московская, д. 5", Capacity = 30, Phone = "+7(492)342-56-78", RentalCostPerHour = 400, Latitude = 55.5728, Longitude = 42.0523 },
                new Venue { Id = 4, Name = "Игровое пространство Ковров", Address = "г. Ковров, пр-т Ленина, д. 36", Capacity = 28, Phone = "+7(492)322-67-89", RentalCostPerHour = 420, Latitude = 56.3572, Longitude = 41.3192 }
            );

            // Seed Players (Владимир и Владимирская область)
            modelBuilder.Entity<Player>().HasData(
                new Player { Id = 1, Name = "Алексей Петров", Email = "alexey.petrov@example.com", Phone = "+7(900)111-22-33", RegisteredDate = new DateTime(2026, 1, 10), BirthDate = new DateTime(1996, 4, 12), City = "Владимир", FavoriteGenre = "Стратегия" },
                new Player { Id = 2, Name = "Мария Соколова", Email = "maria.sokolova@example.com", Phone = "+7(900)222-33-44", RegisteredDate = new DateTime(2026, 1, 11), BirthDate = new DateTime(1998, 8, 5), City = "Суздаль", FavoriteGenre = "Семейная" },
                new Player { Id = 3, Name = "Илья Кузнецов", Email = "ilya.kuznetsov@example.com", Phone = "+7(900)333-44-55", RegisteredDate = new DateTime(2026, 1, 12), BirthDate = new DateTime(1994, 2, 18), City = "Муром", FavoriteGenre = "Карточная" },
                new Player { Id = 4, Name = "Екатерина Орлова", Email = "ekaterina.orlova@example.com", Phone = "+7(900)444-55-66", RegisteredDate = new DateTime(2026, 1, 13), BirthDate = new DateTime(2000, 11, 21), City = "Ковров", FavoriteGenre = "Партийная" },
                new Player { Id = 5, Name = "Дмитрий Фомин", Email = "dmitry.fomin@example.com", Phone = "+7(900)555-66-77", RegisteredDate = new DateTime(2026, 1, 14), BirthDate = new DateTime(1993, 6, 30), City = "Гусь-Хрустальный", FavoriteGenre = "Кооператив" },
                new Player { Id = 6, Name = "Ольга Никитина", Email = "olga.nikitina@example.com", Phone = "+7(900)666-77-88", RegisteredDate = new DateTime(2026, 1, 15), BirthDate = new DateTime(1997, 9, 9), City = "Александров", FavoriteGenre = "Семейная" },
                new Player { Id = 7, Name = "Сергей Власов", Email = "sergey.vlasov@example.com", Phone = "+7(900)777-88-99", RegisteredDate = new DateTime(2026, 1, 16), BirthDate = new DateTime(1991, 12, 1), City = "Боголюбово", FavoriteGenre = "Стратегия" },
                new Player { Id = 8, Name = "Анна Зайцева", Email = "anna.zaitseva@example.com", Phone = "+7(900)888-99-00", RegisteredDate = new DateTime(2026, 1, 17), BirthDate = new DateTime(2001, 3, 14), City = "Мелехово", FavoriteGenre = "Карточная" }
            );

            // Seed Equipment
            modelBuilder.Entity<Equipment>().HasData(
                new Equipment { Id = 1, Name = "Игральные кости (6-гранные)", EquipmentTypeId = 1 },
                new Equipment { Id = 2, Name = "Игровой таймер", EquipmentTypeId = 2 },
                new Equipment { Id = 3, Name = "Игровой коврик", EquipmentTypeId = 3 },
                new Equipment { Id = 4, Name = "Фишки (набор)", EquipmentTypeId = 4 }
            );

            // Seed Games (популярные в России)
            modelBuilder.Entity<Game>().HasData(
                new Game 
                { 
                    Id = 1, 
                    Name = "Монополия", 
                    Description = "Классическая экономическая настольная игра про покупку улиц, аренду и развитие собственности.", 
                    MinPlayers = 2, 
                    MaxPlayers = 6, 
                    AverageDuration = 90, 
                    PublisherId = 1,
                    Complexity = 3,
                    MinAge = 8,
                    YearPublished = 1935
                },
                new Game 
                { 
                    Id = 2, 
                    Name = "Дурак", 
                    Description = "Одна из самых популярных карточных игр в России. Цель - избавиться от карт раньше соперников.", 
                    MinPlayers = 2, 
                    MaxPlayers = 6, 
                    AverageDuration = 30, 
                    PublisherId = 1,
                    Complexity = 1,
                    MinAge = 10,
                    YearPublished = 1810
                },
                new Game 
                { 
                    Id = 3, 
                    Name = "Уно", 
                    Description = "Динамичная карточная игра с цветами и действиями, популярная в компаниях и семьях.", 
                    MinPlayers = 2, 
                    MaxPlayers = 10, 
                    AverageDuration = 25, 
                    PublisherId = 1,
                    Complexity = 1,
                    MinAge = 7,
                    YearPublished = 1971
                },
                new Game 
                { 
                    Id = 4, 
                    Name = "Имаджинариум", 
                    Description = "Популярная российская игра на ассоциации с иллюстрациями.", 
                    MinPlayers = 4, 
                    MaxPlayers = 7, 
                    AverageDuration = 45, 
                    PublisherId = 2,
                    Complexity = 2,
                    MinAge = 12,
                    YearPublished = 2011
                },
                new Game 
                { 
                    Id = 5, 
                    Name = "Мафия", 
                    Description = "Командная психологическая игра на обсуждение и дедукцию.", 
                    MinPlayers = 6, 
                    MaxPlayers = 15, 
                    AverageDuration = 40, 
                    PublisherId = 1,
                    Complexity = 2,
                    MinAge = 12,
                    YearPublished = 1986
                },
                new Game 
                { 
                    Id = 6, 
                    Name = "Свинтус", 
                    Description = "Быстрая карточная игра, очень популярная в российских компаниях.", 
                    MinPlayers = 2, 
                    MaxPlayers = 10, 
                    AverageDuration = 20, 
                    PublisherId = 2,
                    Complexity = 1,
                    MinAge = 8,
                    YearPublished = 2006
                },
                new Game 
                { 
                    Id = 7, 
                    Name = "Кодовые имена", 
                    Description = "Командная игра на ассоциации и дедукцию.", 
                    MinPlayers = 4, 
                    MaxPlayers = 8, 
                    AverageDuration = 20, 
                    PublisherId = 2,
                    Complexity = 2,
                    MinAge = 10,
                    YearPublished = 2015
                },
                new Game 
                { 
                    Id = 8, 
                    Name = "Бункер", 
                    Description = "Популярная в России дискуссионная игра на выживание и социальные роли.", 
                    MinPlayers = 4, 
                    MaxPlayers = 16, 
                    AverageDuration = 45, 
                    PublisherId = 1,
                    Complexity = 3,
                    MinAge = 12,
                    YearPublished = 2018
                }
            );

            modelBuilder.Entity<GameCategoryAssignment>().HasData(
                new GameCategoryAssignment { Id = 1, GameId = 1, GameCategoryId = 1 },
                new GameCategoryAssignment { Id = 2, GameId = 2, GameCategoryId = 4 },
                new GameCategoryAssignment { Id = 3, GameId = 3, GameCategoryId = 4 },
                new GameCategoryAssignment { Id = 4, GameId = 4, GameCategoryId = 5 },
                new GameCategoryAssignment { Id = 5, GameId = 5, GameCategoryId = 5 },
                new GameCategoryAssignment { Id = 6, GameId = 6, GameCategoryId = 4 },
                new GameCategoryAssignment { Id = 7, GameId = 7, GameCategoryId = 5 },
                new GameCategoryAssignment { Id = 8, GameId = 8, GameCategoryId = 5 }
            );

            // Seed available equipment per venue
            modelBuilder.Entity<VenueEquipment>().HasData(
                new VenueEquipment { Id = 1, VenueId = 1, EquipmentId = 1, Quantity = 40 },
                new VenueEquipment { Id = 2, VenueId = 1, EquipmentId = 2, Quantity = 4 },
                new VenueEquipment { Id = 3, VenueId = 1, EquipmentId = 3, Quantity = 2 },
                new VenueEquipment { Id = 4, VenueId = 1, EquipmentId = 4, Quantity = 20 },
                new VenueEquipment { Id = 5, VenueId = 2, EquipmentId = 1, Quantity = 25 },
                new VenueEquipment { Id = 6, VenueId = 2, EquipmentId = 2, Quantity = 2 },
                new VenueEquipment { Id = 7, VenueId = 2, EquipmentId = 4, Quantity = 10 },
                new VenueEquipment { Id = 8, VenueId = 3, EquipmentId = 1, Quantity = 60 },
                new VenueEquipment { Id = 9, VenueId = 3, EquipmentId = 2, Quantity = 8 },
                new VenueEquipment { Id = 10, VenueId = 3, EquipmentId = 3, Quantity = 4 },
                new VenueEquipment { Id = 11, VenueId = 3, EquipmentId = 4, Quantity = 35 },
                new VenueEquipment { Id = 12, VenueId = 4, EquipmentId = 1, Quantity = 30 },
                new VenueEquipment { Id = 13, VenueId = 4, EquipmentId = 2, Quantity = 3 },
                new VenueEquipment { Id = 14, VenueId = 4, EquipmentId = 4, Quantity = 16 }
            );
        }
    }
}
