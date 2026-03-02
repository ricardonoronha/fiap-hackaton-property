using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroSolutions.Properties.Data.Migrations
{
    /// <inheritdoc />
    public partial class adds_farmerid_to_property : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FarmerId",
                table: "properties",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FarmerId",
                table: "properties");
        }
    }
}
