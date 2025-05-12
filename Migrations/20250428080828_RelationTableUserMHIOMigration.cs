using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hatmaker_team2.Migrations
{
    /// <inheritdoc />
    public partial class RelationTableUserMHIOMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserManageHatOrders",
                table: "UserManageHatOrders");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "UserManageHatOrders",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserManageHatOrders",
                table: "UserManageHatOrders",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserManageHatOrders_UserId",
                table: "UserManageHatOrders",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserManageHatOrders",
                table: "UserManageHatOrders");

            migrationBuilder.DropIndex(
                name: "IX_UserManageHatOrders_UserId",
                table: "UserManageHatOrders");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserManageHatOrders");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserManageHatOrders",
                table: "UserManageHatOrders",
                columns: new[] { "UserId", "HatId", "OrderId" });
        }
    }
}
