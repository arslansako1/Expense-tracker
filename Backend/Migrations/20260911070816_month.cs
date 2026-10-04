using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApiProject.Migrations
{
    /// <inheritdoc />
    public partial class month : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Month",
                table: "Budgets",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringRules_AccountId",
                table: "RecurringRules",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringRules_CategoryId",
                table: "RecurringRules",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_RecurringRules_Accounts_AccountId",
                table: "RecurringRules",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RecurringRules_Categories_CategoryId",
                table: "RecurringRules",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecurringRules_Accounts_AccountId",
                table: "RecurringRules");

            migrationBuilder.DropForeignKey(
                name: "FK_RecurringRules_Categories_CategoryId",
                table: "RecurringRules");

            migrationBuilder.DropIndex(
                name: "IX_RecurringRules_AccountId",
                table: "RecurringRules");

            migrationBuilder.DropIndex(
                name: "IX_RecurringRules_CategoryId",
                table: "RecurringRules");

            migrationBuilder.DropColumn(
                name: "Month",
                table: "Budgets");
        }
    }
}
