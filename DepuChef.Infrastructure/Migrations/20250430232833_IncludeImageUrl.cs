using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DepuChef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IncludeImageUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Recipes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecipeImageUrl",
                table: "RecipeProcesses",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "RecipeImageUrl",
                table: "RecipeProcesses");
        }
    }
}
