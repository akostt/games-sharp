using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GamesSharp.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Equipments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GameCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    RegisteredDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    City = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FavoriteGenre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Publishers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FoundedYear = table.Column<int>(type: "int", nullable: true),
                    Website = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Publishers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Venues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    RentalCostPerHour = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Venues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Achievements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AchievedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Achievements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Achievements_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    MinPlayers = table.Column<int>(type: "int", nullable: false),
                    MaxPlayers = table.Column<int>(type: "int", nullable: false),
                    AverageDuration = table.Column<int>(type: "int", nullable: false),
                    Complexity = table.Column<int>(type: "int", nullable: true),
                    MinAge = table.Column<int>(type: "int", nullable: true),
                    YearPublished = table.Column<int>(type: "int", nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    PublisherId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Games_GameCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "GameCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Games_Publishers_PublisherId",
                        column: x => x.PublisherId,
                        principalTable: "Publishers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "GameEquipments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    EquipmentId = table.Column<int>(type: "int", nullable: false),
                    RequiredQuantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameEquipments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameEquipments_Equipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameEquipments_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    ReviewText = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ReviewDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameReviews_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameReviews_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    VenueId = table.Column<int>(type: "int", nullable: true),
                    ScheduledDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualStartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualEndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Organizer = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MaxParticipants = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameSessions_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GameSessions_Venues_VenueId",
                        column: x => x.VenueId,
                        principalTable: "Venues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "SessionPlayers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameSessionId = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: true),
                    IsWinner = table.Column<bool>(type: "bit", nullable: false),
                    Place = table.Column<int>(type: "int", nullable: true),
                    Team = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionPlayers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SessionPlayers_GameSessions_GameSessionId",
                        column: x => x.GameSessionId,
                        principalTable: "GameSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SessionPlayers_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Equipments",
                columns: new[] { "Id", "Description", "Name", "Quantity", "Type" },
                values: new object[,]
                {
                    { 1, null, "Игральные кости (6-гранные)", 100, "Кости" },
                    { 2, null, "Игровой таймер", 10, "Таймер" },
                    { 3, null, "Игровой коврик", 5, "Аксессуар" },
                    { 4, null, "Фишки (набор)", 50, "Фишки" }
                });

            migrationBuilder.InsertData(
                table: "GameCategories",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Игры, требующие стратегического мышления", "Стратегия" },
                    { 2, "Игры для всей семьи", "Семейная" },
                    { 3, "Совместные игры", "Кооператив" },
                    { 4, "Карточные игры", "Карточная" },
                    { 5, "Легкие игры для компании", "Партийная" }
                });

            migrationBuilder.InsertData(
                table: "Publishers",
                columns: new[] { "Id", "Country", "FoundedYear", "Name", "Website" },
                values: new object[,]
                {
                    { 1, "Россия", 2009, "Hobby World", "https://www.hobbyworld.ru" },
                    { 2, "Россия", 2011, "Cosmodrome Games", "https://cosmodrome.games" },
                    { 3, "США", 2002, "Days of Wonder", "https://www.daysofwonder.com" }
                });

            migrationBuilder.InsertData(
                table: "Venues",
                columns: new[] { "Id", "Address", "Capacity", "Description", "Name", "Phone", "RentalCostPerHour" },
                values: new object[,]
                {
                    { 1, "ул. Арбат, д. 10", 30, null, "Игротека на Арбате", "+7(495)123-45-67", 500m },
                    { 2, "Ленинский пр-т, д. 5", 20, null, "Антикафе Таймкод", "+7(495)234-56-78", 300m },
                    { 3, "пр-т Мира, д. 33", 50, null, "Клуб настольных игр Мосигра", "+7(495)345-67-89", 800m }
                });

            migrationBuilder.InsertData(
                table: "Games",
                columns: new[] { "Id", "AverageDuration", "CategoryId", "Complexity", "Description", "MaxPlayers", "MinAge", "MinPlayers", "Name", "PublisherId", "YearPublished" },
                values: new object[,]
                {
                    { 1, 60, 4, 3, "Пародийная карточная игра на тему фэнтези-подземелий. Убивай монстров, предавай друзей, хватай сокровища!", 6, 10, 3, "Манчкин", 1, 2001 },
                    { 2, 45, 5, 2, "Российская игра на ассоциации с красивыми иллюстрациями. Придумывай ассоциации к картинкам!", 7, 12, 4, "Имаджинариум", 1, 2011 },
                    { 3, 15, 4, 1, "Весёлая карточная игра в русскую рулетку с котятами, лазерами и козами", 5, 7, 2, "Взрывные котята", 1, 2015 },
                    { 4, 15, 5, 2, "Командная игра на ассоциации и дедукцию. Угадывай слова по намёкам капитана!", 8, 10, 4, "Кодовые имена", 2, 2015 },
                    { 5, 75, 1, 6, "Классическая стратегия о колонизации острова Катан. Торгуй, строй, побеждай!", 4, 10, 3, "Колонизаторы", 1, 1995 },
                    { 6, 45, 5, 1, "Популярная игра в объяснение слов. Объясняй слова, не используя однокоренные!", 12, 7, 4, "Элиас", 2, 1993 },
                    { 7, 30, 4, 1, "Знаменитая карточная игра. Избавься от всех карт первым!", 10, 7, 2, "Uno", 2, 1971 },
                    { 8, 40, 1, 4, "Игра на выкладывание тайлов средневекового французского ландшафта", 5, 8, 2, "Каркассон", 1, 2000 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Achievements_PlayerId",
                table: "Achievements",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_GameEquipments_EquipmentId",
                table: "GameEquipments",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_GameEquipments_GameId",
                table: "GameEquipments",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GameReviews_GameId",
                table: "GameReviews",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GameReviews_PlayerId",
                table: "GameReviews",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_CategoryId",
                table: "Games",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_PublisherId",
                table: "Games",
                column: "PublisherId");

            migrationBuilder.CreateIndex(
                name: "IX_GameSessions_GameId",
                table: "GameSessions",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GameSessions_VenueId",
                table: "GameSessions",
                column: "VenueId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionPlayers_GameSessionId",
                table: "SessionPlayers",
                column: "GameSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionPlayers_PlayerId",
                table: "SessionPlayers",
                column: "PlayerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Achievements");

            migrationBuilder.DropTable(
                name: "GameEquipments");

            migrationBuilder.DropTable(
                name: "GameReviews");

            migrationBuilder.DropTable(
                name: "SessionPlayers");

            migrationBuilder.DropTable(
                name: "Equipments");

            migrationBuilder.DropTable(
                name: "GameSessions");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Venues");

            migrationBuilder.DropTable(
                name: "GameCategories");

            migrationBuilder.DropTable(
                name: "Publishers");
        }
    }
}
