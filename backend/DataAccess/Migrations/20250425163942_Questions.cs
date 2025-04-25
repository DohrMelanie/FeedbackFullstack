using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Questions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Text",
                table: "Feedbacks");

            migrationBuilder.AddColumn<int>(
                name: "Helpful",
                table: "Feedbacks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Knowledgeable",
                table: "Feedbacks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "LikedLeast",
                table: "Feedbacks",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LikedMost",
                table: "Feedbacks",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Satisfied",
                table: "Feedbacks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Helpful",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "Knowledgeable",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "LikedLeast",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "LikedMost",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "Satisfied",
                table: "Feedbacks");

            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "Feedbacks",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
