using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hatmaker_team2.Migrations
{
    /// <inheritdoc />
    public partial class DeactivateHats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeactivated",
                table: "Hats",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeactivated",
                table: "Hats");
        }
    }
}
