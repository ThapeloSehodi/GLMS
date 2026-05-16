using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GLMS.Migrations
{
    /// <inheritdoc />
    public partial class MultiCurrencyUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CostUSD",
                table: "ServiceRequests",
                newName: "ForeignAmount");

            migrationBuilder.AddColumn<string>(
                name: "CurrencyType",
                table: "ServiceRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrencyType",
                table: "ServiceRequests");

            migrationBuilder.RenameColumn(
                name: "ForeignAmount",
                table: "ServiceRequests",
                newName: "CostUSD");
        }
    }
}
