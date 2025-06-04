using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymMate.Migrations
{
    /// <inheritdoc />
    public partial class addExerImg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "ExerciseImg",
                table: "Exercises",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExerciseImg",
                table: "Exercises");
        }
    }
}
