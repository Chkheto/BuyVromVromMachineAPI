using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyVromVromMachineAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddRolesAndCarOwner : Migration
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
