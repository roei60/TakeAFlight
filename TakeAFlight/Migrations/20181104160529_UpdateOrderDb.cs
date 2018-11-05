using Microsoft.EntityFrameworkCore.Migrations;

namespace TakeAFlight.Migrations
{
    public partial class UpdateOrderDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TicketNumber",
                table: "FlightOrders");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "FlightOrders",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                table: "Destinations",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "Destinations",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "FlightOrders");

            migrationBuilder.AddColumn<string>(
                name: "TicketNumber",
                table: "FlightOrders",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                table: "Destinations",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "Destinations",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
