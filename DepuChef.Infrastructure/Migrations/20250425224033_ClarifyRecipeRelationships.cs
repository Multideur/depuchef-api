using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DepuChef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ClarifyRecipeRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Note_Recipes_RecipeId",
                table: "Note");

            migrationBuilder.AlterColumn<Guid>(
                name: "RecipeId",
                table: "Note",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Note_Recipes_RecipeId",
                table: "Note",
                column: "RecipeId",
                principalTable: "Recipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Note_Recipes_RecipeId",
                table: "Note");

            migrationBuilder.AlterColumn<Guid>(
                name: "RecipeId",
                table: "Note",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Note_Recipes_RecipeId",
                table: "Note",
                column: "RecipeId",
                principalTable: "Recipes",
                principalColumn: "Id");
        }
    }
}
