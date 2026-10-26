using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyVromVromMachineAPI.Migrations
{
    /// <inheritdoc />
    public partial class SyncModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Users",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Buyer");

            migrationBuilder.AddColumn<int>(
                name: "SellerId",
                table: "Cars",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cars_SellerId",
                table: "Cars",
                column: "SellerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_Users_SellerId",
                table: "Cars",
                column: "SellerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cars_Users_SellerId",
                table: "Cars");

            migrationBuilder.DropIndex(
                name: "IX_Cars_SellerId",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SellerId",
                table: "Cars");
        }
    }
}
