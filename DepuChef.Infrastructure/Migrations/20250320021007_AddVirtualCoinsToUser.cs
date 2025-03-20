using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DepuChef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVirtualCoinsToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VirtualCoins",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VirtualCoins",
                table: "Users");
        }
    }
}
