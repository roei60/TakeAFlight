using Microsoft.EntityFrameworkCore.Migrations;

namespace TakeAFlight.Migrations
{
    public partial class UpdatePassengerDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Passengers_ApplicationUser_UserId",
                table: "Passengers");

            migrationBuilder.DropIndex(
                name: "IX_Passengers_UserId",
                table: "Passengers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Passengers");

            migrationBuilder.RenameColumn(
                name: "PassengerID",
                table: "Passengers",
                newName: "ID");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Passengers",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserID",
                table: "Passengers",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "IdPassenger",
                table: "Passengers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Passengers_ApplicationUserID",
                table: "Passengers",
                column: "ApplicationUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Passengers_ApplicationUser_ApplicationUserID",
                table: "Passengers",
                column: "ApplicationUserID",
                principalTable: "ApplicationUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Passengers_ApplicationUser_ApplicationUserID",
                table: "Passengers");

            migrationBuilder.DropIndex(
                name: "IX_Passengers_ApplicationUserID",
                table: "Passengers");

            migrationBuilder.DropColumn(
                name: "IdPassenger",
                table: "Passengers");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Passengers",
                newName: "PassengerID");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Passengers",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ApplicationUserID",
                table: "Passengers",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Passengers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Passengers_UserId",
                table: "Passengers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Passengers_ApplicationUser_UserId",
                table: "Passengers",
                column: "UserId",
                principalTable: "ApplicationUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
