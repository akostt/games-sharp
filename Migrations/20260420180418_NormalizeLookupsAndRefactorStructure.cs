using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GamesSharp.Migrations
{
    /// <inheritdoc />
    public partial class NormalizeLookupsAndRefactorStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CountryId",
                table: "Publishers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SessionStatusId",
                table: "GameSessions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "EquipmentTypeId",
                table: "Equipments",
                type: "INTEGER",
                nullable: true);

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
                table: "SessionStatuses",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[,]
                {
                    { 1, "scheduled", "Запланирована" },
                    { 2, "in_progress", "В процессе" },
                    { 3, "completed", "Завершена" },
                    { 4, "cancelled", "Отменена" }
                });

            migrationBuilder.Sql(@"
                UPDATE Publishers
                SET CountryId = CASE Country
                    WHEN 'Россия' THEN 1
                    WHEN 'США' THEN 2
                    ELSE NULL
                END;");

            migrationBuilder.Sql(@"
                UPDATE Equipments
                SET EquipmentTypeId = CASE Type
                    WHEN 'Кости' THEN 1
                    WHEN 'Таймер' THEN 2
                    WHEN 'Аксессуар' THEN 3
                    WHEN 'Фишки' THEN 4
                    ELSE NULL
                END;");

            migrationBuilder.Sql(@"
                UPDATE GameSessions
                SET SessionStatusId = CASE Status
                    WHEN 'Запланирована' THEN 1
                    WHEN 'В процессе' THEN 2
                    WHEN 'Завершена' THEN 3
                    WHEN 'Отменена' THEN 4
                    ELSE 1
                END;");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Publishers");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "GameSessions");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Equipments");

            migrationBuilder.CreateIndex(
                name: "IX_Publishers_CountryId",
                table: "Publishers",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_GameSessions_SessionStatusId",
                table: "GameSessions",
                column: "SessionStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_EquipmentTypeId",
                table: "Equipments",
                column: "EquipmentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Name",
                table: "Countries",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentTypes_Name",
                table: "EquipmentTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SessionStatuses_Code",
                table: "SessionStatuses",
                column: "Code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Equipments_EquipmentTypes_EquipmentTypeId",
                table: "Equipments",
                column: "EquipmentTypeId",
                principalTable: "EquipmentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_GameSessions_SessionStatuses_SessionStatusId",
                table: "GameSessions",
                column: "SessionStatusId",
                principalTable: "SessionStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Publishers_Countries_CountryId",
                table: "Publishers",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipments_EquipmentTypes_EquipmentTypeId",
                table: "Equipments");

            migrationBuilder.DropForeignKey(
                name: "FK_GameSessions_SessionStatuses_SessionStatusId",
                table: "GameSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Publishers_Countries_CountryId",
                table: "Publishers");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Publishers",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "GameSessions",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                defaultValue: "Запланирована");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Equipments",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE Publishers
                SET Country = (
                    SELECT c.Name
                    FROM Countries c
                    WHERE c.Id = Publishers.CountryId
                );");

            migrationBuilder.Sql(@"
                UPDATE Equipments
                SET Type = (
                    SELECT et.Name
                    FROM EquipmentTypes et
                    WHERE et.Id = Equipments.EquipmentTypeId
                );");

            migrationBuilder.Sql(@"
                UPDATE GameSessions
                SET Status = (
                    SELECT ss.Name
                    FROM SessionStatuses ss
                    WHERE ss.Id = GameSessions.SessionStatusId
                );");

            migrationBuilder.DropIndex(
                name: "IX_Publishers_CountryId",
                table: "Publishers");

            migrationBuilder.DropIndex(
                name: "IX_GameSessions_SessionStatusId",
                table: "GameSessions");

            migrationBuilder.DropIndex(
                name: "IX_Equipments_EquipmentTypeId",
                table: "Equipments");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "Publishers");

            migrationBuilder.DropColumn(
                name: "SessionStatusId",
                table: "GameSessions");

            migrationBuilder.DropColumn(
                name: "EquipmentTypeId",
                table: "Equipments");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "EquipmentTypes");

            migrationBuilder.DropTable(
                name: "SessionStatuses");

        }
    }
}
