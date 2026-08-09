using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinTrack_II_Trimestre.Migrations
{
    /// <inheritdoc />
    public partial class BudgetPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "BudgetPlans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BudgetPlans_UserId",
                table: "BudgetPlans",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetPlans_Users_UserId",
                table: "BudgetPlans",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetPlans_Users_UserId",
                table: "BudgetPlans");

            migrationBuilder.DropIndex(
                name: "IX_BudgetPlans_UserId",
                table: "BudgetPlans");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "BudgetPlans");
        }
    }
}
