using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hatmaker_team2.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedIdInOrderContainsHatsMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderContainsHats",
                table: "OrderContainsHats");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "OrderContainsHats",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderContainsHats",
                table: "OrderContainsHats",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_OrderContainsHats_OrderId",
                table: "OrderContainsHats",
                column: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderContainsHats",
                table: "OrderContainsHats");

            migrationBuilder.DropIndex(
                name: "IX_OrderContainsHats_OrderId",
                table: "OrderContainsHats");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "OrderContainsHats");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderContainsHats",
                table: "OrderContainsHats",
                columns: new[] { "OrderId", "HatId" });
        }
    }
}
