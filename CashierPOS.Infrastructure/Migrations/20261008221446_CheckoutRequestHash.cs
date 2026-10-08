using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CashierPOS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CheckoutRequestHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RequestHash",
                table: "Sale",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestHash",
                table: "Sale");
        }
    }
}
