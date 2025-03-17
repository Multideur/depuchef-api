using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DepuChef.Infrastructure.Migrations;

/// <inheritdoc />
public partial class UpdateSchemaWithSubstitutions : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "CaloriesAfterSubstitution",
            table: "Ingredient",
            type: "int",
            nullable: true);

        migrationBuilder.CreateTable(
            name: "HealthySubstitution",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                IngredientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Original = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Substitute = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_HealthySubstitution", x => x.Id);
                table.ForeignKey(
                    name: "FK_HealthySubstitution_Ingredient_IngredientId",
                    column: x => x.IngredientId,
                    principalTable: "Ingredient",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_HealthySubstitution_IngredientId",
            table: "HealthySubstitution",
            column: "IngredientId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "HealthySubstitution");

        migrationBuilder.DropColumn(
            name: "CaloriesAfterSubstitution",
            table: "Ingredient");
    }
}
