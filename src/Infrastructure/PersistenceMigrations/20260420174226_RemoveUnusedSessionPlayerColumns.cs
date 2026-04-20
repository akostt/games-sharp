using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GamesSharp.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnusedSessionPlayerColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comment",
                table: "SessionPlayers");

            migrationBuilder.DropColumn(
                name: "Place",
                table: "SessionPlayers");

            migrationBuilder.DropColumn(
                name: "Team",
                table: "SessionPlayers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "SessionPlayers",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Place",
                table: "SessionPlayers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Team",
                table: "SessionPlayers",
                type: "TEXT",
                maxLength: 50,
                nullable: true);
        }
    }
}
