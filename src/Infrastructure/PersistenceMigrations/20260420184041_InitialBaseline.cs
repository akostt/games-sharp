using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GamesSharp.src.Infrastructure.PersistenceMigrations
{
    /// <inheritdoc />
    public partial class InitialBaseline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GameCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    RegisteredDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    City = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    FavoriteGenre = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SessionStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Venues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Capacity = table.Column<int>(type: "INTEGER", nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    RentalCostPerHour = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Latitude = table.Column<double>(type: "REAL", nullable: true),
                    Longitude = table.Column<double>(type: "REAL", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Venues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Publishers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CountryId = table.Column<int>(type: "INTEGER", nullable: true),
                    FoundedYear = table.Column<int>(type: "INTEGER", nullable: true),
                    Website = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Publishers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Publishers_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Equipments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    EquipmentTypeId = table.Column<int>(type: "INTEGER", nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Equipments_EquipmentTypes_EquipmentTypeId",
                        column: x => x.EquipmentTypeId,
                        principalTable: "EquipmentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    MinPlayers = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxPlayers = table.Column<int>(type: "INTEGER", nullable: false),
                    AverageDuration = table.Column<int>(type: "INTEGER", nullable: false),
                    Complexity = table.Column<int>(type: "INTEGER", nullable: true),
                    MinAge = table.Column<int>(type: "INTEGER", nullable: true),
                    YearPublished = table.Column<int>(type: "INTEGER", nullable: true),
                    PublisherId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Games_Publishers_PublisherId",
                        column: x => x.PublisherId,
                        principalTable: "Publishers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "VenueEquipments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VenueId = table.Column<int>(type: "INTEGER", nullable: false),
                    EquipmentId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VenueEquipments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VenueEquipments_Equipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VenueEquipments_Venues_VenueId",
                        column: x => x.VenueId,
                        principalTable: "Venues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameCategoryAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameId = table.Column<int>(type: "INTEGER", nullable: false),
                    GameCategoryId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameCategoryAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameCategoryAssignments_GameCategories_GameCategoryId",
                        column: x => x.GameCategoryId,
                        principalTable: "GameCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameCategoryAssignments_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameEquipments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameId = table.Column<int>(type: "INTEGER", nullable: false),
                    EquipmentId = table.Column<int>(type: "INTEGER", nullable: false),
                    RequiredQuantity = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameEquipments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameEquipments_Equipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GameEquipments_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameId = table.Column<int>(type: "INTEGER", nullable: false),
                    VenueId = table.Column<int>(type: "INTEGER", nullable: true),
                    ScheduledDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ActualStartTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ActualEndTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SessionStatusId = table.Column<int>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Organizer = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    MaxParticipants = table.Column<int>(type: "INTEGER", nullable: true)
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
                        name: "FK_GameSessions_SessionStatuses_SessionStatusId",
                        column: x => x.SessionStatusId,
                        principalTable: "SessionStatuses",
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
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameSessionId = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayerId = table.Column<int>(type: "INTEGER", nullable: false),
                    Score = table.Column<int>(type: "INTEGER", nullable: true),
                    IsWinner = table.Column<bool>(type: "INTEGER", nullable: false)
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
                table: "Countries",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Россия" },
                    { 2, "США" }
                });

            migrationBuilder.InsertData(
                table: "EquipmentTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Кости" },
                    { 2, "Таймер" },
                    { 3, "Аксессуар" },
                    { 4, "Фишки" }
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
                table: "Players",
                columns: new[] { "Id", "BirthDate", "City", "Email", "FavoriteGenre", "Name", "Phone", "RegisteredDate" },
                values: new object[,]
                {
                    { 1, new DateTime(1996, 4, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Владимир", "alexey.petrov@example.com", "Стратегия", "Алексей Петров", "+7(900)111-22-33", new DateTime(2026, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, new DateTime(1998, 8, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Суздаль", "maria.sokolova@example.com", "Семейная", "Мария Соколова", "+7(900)222-33-44", new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, new DateTime(1994, 2, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Муром", "ilya.kuznetsov@example.com", "Карточная", "Илья Кузнецов", "+7(900)333-44-55", new DateTime(2026, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, new DateTime(2000, 11, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ковров", "ekaterina.orlova@example.com", "Партийная", "Екатерина Орлова", "+7(900)444-55-66", new DateTime(2026, 1, 13, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, new DateTime(1993, 6, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Гусь-Хрустальный", "dmitry.fomin@example.com", "Кооператив", "Дмитрий Фомин", "+7(900)555-66-77", new DateTime(2026, 1, 14, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6, new DateTime(1997, 9, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "Александров", "olga.nikitina@example.com", "Семейная", "Ольга Никитина", "+7(900)666-77-88", new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 7, new DateTime(1991, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Боголюбово", "sergey.vlasov@example.com", "Стратегия", "Сергей Власов", "+7(900)777-88-99", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 8, new DateTime(2001, 3, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Мелехово", "anna.zaitseva@example.com", "Карточная", "Анна Зайцева", "+7(900)888-99-00", new DateTime(2026, 1, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "SessionStatuses",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[,]
                {
                    { 1, "scheduled", "Запланирована" },
                    { 2, "in_progress", "В процессе" },
                    { 3, "completed", "Завершена" },
                    { 4, "cancelled", "Отменена" }
                });

            migrationBuilder.InsertData(
                table: "Venues",
                columns: new[] { "Id", "Address", "Capacity", "Description", "Latitude", "Longitude", "Name", "Phone", "RentalCostPerHour" },
                values: new object[,]
                {
                    { 1, "г. Владимир, ул. Большая Московская, д. 11", 40, null, 56.129100000000001, 40.406599999999997, "Клуб настольных игр Владимир", "+7(492)212-34-56", 600m },
                    { 2, "г. Суздаль, ул. Ленина, д. 72", 24, null, 56.419499999999999, 40.452500000000001, "Игротека Суздаль", "+7(492)312-45-67", 450m },
                    { 3, "г. Муром, ул. Московская, д. 5", 30, null, 55.572800000000001, 42.052300000000002, "Антикафе Муром", "+7(492)342-56-78", 400m },
                    { 4, "г. Ковров, пр-т Ленина, д. 36", 28, null, 56.357199999999999, 41.319200000000002, "Игровое пространство Ковров", "+7(492)322-67-89", 420m }
                });

            migrationBuilder.InsertData(
                table: "Equipments",
                columns: new[] { "Id", "Description", "EquipmentTypeId", "Name" },
                values: new object[,]
                {
                    { 1, null, 1, "Игральные кости (6-гранные)" },
                    { 2, null, 2, "Игровой таймер" },
                    { 3, null, 3, "Игровой коврик" },
                    { 4, null, 4, "Фишки (набор)" }
                });

            migrationBuilder.InsertData(
                table: "Publishers",
                columns: new[] { "Id", "CountryId", "FoundedYear", "Name", "Website" },
                values: new object[,]
                {
                    { 1, 1, 2009, "Hobby World", "https://www.hobbyworld.ru" },
                    { 2, 1, 2011, "Cosmodrome Games", "https://cosmodrome.games" },
                    { 3, 2, 2002, "Days of Wonder", "https://www.daysofwonder.com" }
                });

            migrationBuilder.InsertData(
                table: "Games",
                columns: new[] { "Id", "AverageDuration", "Complexity", "Description", "MaxPlayers", "MinAge", "MinPlayers", "Name", "PublisherId", "YearPublished" },
                values: new object[,]
                {
                    { 1, 90, 3, "Классическая экономическая настольная игра про покупку улиц, аренду и развитие собственности.", 6, 8, 2, "Монополия", 1, 1935 },
                    { 2, 30, 1, "Одна из самых популярных карточных игр в России. Цель - избавиться от карт раньше соперников.", 6, 10, 2, "Дурак", 1, 1810 },
                    { 3, 25, 1, "Динамичная карточная игра с цветами и действиями, популярная в компаниях и семьях.", 10, 7, 2, "Уно", 1, 1971 },
                    { 4, 45, 2, "Популярная российская игра на ассоциации с иллюстрациями.", 7, 12, 4, "Имаджинариум", 2, 2011 },
                    { 5, 40, 2, "Командная психологическая игра на обсуждение и дедукцию.", 15, 12, 6, "Мафия", 1, 1986 },
                    { 6, 20, 1, "Быстрая карточная игра, очень популярная в российских компаниях.", 10, 8, 2, "Свинтус", 2, 2006 },
                    { 7, 20, 2, "Командная игра на ассоциации и дедукцию.", 8, 10, 4, "Кодовые имена", 2, 2015 },
                    { 8, 45, 3, "Популярная в России дискуссионная игра на выживание и социальные роли.", 16, 12, 4, "Бункер", 1, 2018 }
                });

            migrationBuilder.InsertData(
                table: "VenueEquipments",
                columns: new[] { "Id", "EquipmentId", "Quantity", "VenueId" },
                values: new object[,]
                {
                    { 1, 1, 40, 1 },
                    { 2, 2, 4, 1 },
                    { 3, 3, 2, 1 },
                    { 4, 4, 20, 1 },
                    { 5, 1, 25, 2 },
                    { 6, 2, 2, 2 },
                    { 7, 4, 10, 2 },
                    { 8, 1, 60, 3 },
                    { 9, 2, 8, 3 },
                    { 10, 3, 4, 3 },
                    { 11, 4, 35, 3 },
                    { 12, 1, 30, 4 },
                    { 13, 2, 3, 4 },
                    { 14, 4, 16, 4 }
                });

            migrationBuilder.InsertData(
                table: "GameCategoryAssignments",
                columns: new[] { "Id", "GameCategoryId", "GameId" },
                values: new object[,]
                {
                    { 1, 1, 1 },
                    { 2, 4, 2 },
                    { 3, 4, 3 },
                    { 4, 5, 4 },
                    { 5, 5, 5 },
                    { 6, 4, 6 },
                    { 7, 5, 7 },
                    { 8, 5, 8 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Name",
                table: "Countries",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_EquipmentTypeId",
                table: "Equipments",
                column: "EquipmentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentTypes_Name",
                table: "EquipmentTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameCategoryAssignments_GameCategoryId",
                table: "GameCategoryAssignments",
                column: "GameCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_GameCategoryAssignments_GameId",
                table: "GameCategoryAssignments",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GameEquipments_EquipmentId",
                table: "GameEquipments",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_GameEquipments_GameId",
                table: "GameEquipments",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_PublisherId",
                table: "Games",
                column: "PublisherId");

            migrationBuilder.CreateIndex(
                name: "IX_GameSessions_GameId",
                table: "GameSessions",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GameSessions_SessionStatusId",
                table: "GameSessions",
                column: "SessionStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_GameSessions_VenueId",
                table: "GameSessions",
                column: "VenueId");

            migrationBuilder.CreateIndex(
                name: "IX_Publishers_CountryId",
                table: "Publishers",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionPlayers_GameSessionId",
                table: "SessionPlayers",
                column: "GameSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionPlayers_PlayerId",
                table: "SessionPlayers",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionStatuses_Code",
                table: "SessionStatuses",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VenueEquipments_EquipmentId",
                table: "VenueEquipments",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_VenueEquipments_VenueId_EquipmentId",
                table: "VenueEquipments",
                columns: new[] { "VenueId", "EquipmentId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameCategoryAssignments");

            migrationBuilder.DropTable(
                name: "GameEquipments");

            migrationBuilder.DropTable(
                name: "SessionPlayers");

            migrationBuilder.DropTable(
                name: "VenueEquipments");

            migrationBuilder.DropTable(
                name: "GameCategories");

            migrationBuilder.DropTable(
                name: "GameSessions");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Equipments");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "SessionStatuses");

            migrationBuilder.DropTable(
                name: "Venues");

            migrationBuilder.DropTable(
                name: "EquipmentTypes");

            migrationBuilder.DropTable(
                name: "Publishers");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
