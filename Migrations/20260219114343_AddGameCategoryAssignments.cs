using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GamesSharp.Migrations
{
    /// <inheritdoc />
    public partial class AddGameCategoryAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_GameCategories_CategoryId",
                table: "Games");

            migrationBuilder.DropIndex(
                name: "IX_Games_CategoryId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Games");

            migrationBuilder.AlterColumn<decimal>(
                name: "RentalCostPerHour",
                table: "Venues",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "GameCategoryAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    GameCategoryId = table.Column<int>(type: "int", nullable: false)
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

            migrationBuilder.InsertData(
                table: "GameCategoryAssignments",
                columns: new[] { "Id", "GameCategoryId", "GameId" },
                values: new object[,]
                {
                    { 1, 4, 1 },
                    { 2, 5, 2 },
                    { 3, 4, 3 },
                    { 4, 5, 4 },
                    { 5, 1, 5 },
                    { 6, 5, 6 },
                    { 7, 4, 7 },
                    { 8, 1, 8 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameCategoryAssignments_GameCategoryId",
                table: "GameCategoryAssignments",
                column: "GameCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_GameCategoryAssignments_GameId",
                table: "GameCategoryAssignments",
                column: "GameId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameCategoryAssignments");

            migrationBuilder.AlterColumn<decimal>(
                name: "RentalCostPerHour",
                table: "Venues",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Games",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 1,
                column: "CategoryId",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 2,
                column: "CategoryId",
                value: 5);

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 3,
                column: "CategoryId",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 4,
                column: "CategoryId",
                value: 5);

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 5,
                column: "CategoryId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 6,
                column: "CategoryId",
                value: 5);

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 7,
                column: "CategoryId",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 8,
                column: "CategoryId",
                value: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Games_CategoryId",
                table: "Games",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_GameCategories_CategoryId",
                table: "Games",
                column: "CategoryId",
                principalTable: "GameCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
