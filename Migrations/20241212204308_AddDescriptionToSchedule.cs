using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proiect_MPA.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionToSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Schedule",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Schedule");
        }
    }
}
