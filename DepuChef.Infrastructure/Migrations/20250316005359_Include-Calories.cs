using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DepuChef.Infrastructure.Migrations;

/// <inheritdoc />
public partial class IncludeCalories : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Ingredient_Recipe_RecipeId",
            table: "Ingredient");

        migrationBuilder.DropForeignKey(
            name: "FK_Instruction_Recipe_RecipeId",
            table: "Instruction");

        migrationBuilder.DropForeignKey(
            name: "FK_Note_Recipe_RecipeId",
            table: "Note");

        migrationBuilder.DropPrimaryKey(
            name: "PK_Recipe",
            table: "Recipe");

        migrationBuilder.RenameTable(
            name: "Recipe",
            newName: "Recipes");

        migrationBuilder.AddColumn<Guid>(
            name: "UserId",
            table: "RecipeProcesses",
            type: "uniqueidentifier",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.AddColumn<int>(
            name: "Calories",
            table: "IngredientItem",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "Calories",
            table: "Ingredient",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "Calories",
            table: "Recipes",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "CaloriesAfterSubstitution",
            table: "Recipes",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<Guid>(
            name: "UserId",
            table: "Recipes",
            type: "uniqueidentifier",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.AddPrimaryKey(
            name: "PK_Recipes",
            table: "Recipes",
            column: "Id");

        migrationBuilder.CreateIndex(
            name: "IX_Recipes_UserId",
            table: "Recipes",
            column: "UserId");

        migrationBuilder.AddForeignKey(
            name: "FK_Ingredient_Recipes_RecipeId",
            table: "Ingredient",
            column: "RecipeId",
            principalTable: "Recipes",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_Instruction_Recipes_RecipeId",
            table: "Instruction",
            column: "RecipeId",
            principalTable: "Recipes",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_Note_Recipes_RecipeId",
            table: "Note",
            column: "RecipeId",
            principalTable: "Recipes",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_Recipes_Users_UserId",
            table: "Recipes",
            column: "UserId",
            principalTable: "Users",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Ingredient_Recipes_RecipeId",
            table: "Ingredient");

        migrationBuilder.DropForeignKey(
            name: "FK_Instruction_Recipes_RecipeId",
            table: "Instruction");

        migrationBuilder.DropForeignKey(
            name: "FK_Note_Recipes_RecipeId",
            table: "Note");

        migrationBuilder.DropForeignKey(
            name: "FK_Recipes_Users_UserId",
            table: "Recipes");

        migrationBuilder.DropPrimaryKey(
            name: "PK_Recipes",
            table: "Recipes");

        migrationBuilder.DropIndex(
            name: "IX_Recipes_UserId",
            table: "Recipes");

        migrationBuilder.DropColumn(
            name: "UserId",
            table: "RecipeProcesses");

        migrationBuilder.DropColumn(
            name: "Calories",
            table: "IngredientItem");

        migrationBuilder.DropColumn(
            name: "Calories",
            table: "Ingredient");

        migrationBuilder.DropColumn(
            name: "Calories",
            table: "Recipes");

        migrationBuilder.DropColumn(
            name: "CaloriesAfterSubstitution",
            table: "Recipes");

        migrationBuilder.DropColumn(
            name: "UserId",
            table: "Recipes");

        migrationBuilder.RenameTable(
            name: "Recipes",
            newName: "Recipe");

        migrationBuilder.AddPrimaryKey(
            name: "PK_Recipe",
            table: "Recipe",
            column: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_Ingredient_Recipe_RecipeId",
            table: "Ingredient",
            column: "RecipeId",
            principalTable: "Recipe",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_Instruction_Recipe_RecipeId",
            table: "Instruction",
            column: "RecipeId",
            principalTable: "Recipe",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_Note_Recipe_RecipeId",
            table: "Note",
            column: "RecipeId",
            principalTable: "Recipe",
            principalColumn: "Id");
    }
}
