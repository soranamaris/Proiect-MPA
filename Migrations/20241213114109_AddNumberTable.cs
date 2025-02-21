using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proiect_MPA.Migrations
{
    /// <inheritdoc />
    public partial class AddNumberTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberTable",
                table: "Table",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberTable",
                table: "Table");
        }
    }
}
