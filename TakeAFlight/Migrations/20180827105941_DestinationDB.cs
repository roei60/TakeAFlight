using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TakeAFlight.Migrations
{
    public partial class DestinationDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flight_Destination_DestinationID",
                table: "Flight");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Destination",
                table: "Destination");

            migrationBuilder.RenameTable(
                name: "Destination",
                newName: "Destinations");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Destinations",
                table: "Destinations",
                column: "DestinationID");

            migrationBuilder.AddForeignKey(
                name: "FK_Flight_Destinations_DestinationID",
                table: "Flight",
                column: "DestinationID",
                principalTable: "Destinations",
                principalColumn: "DestinationID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flight_Destinations_DestinationID",
                table: "Flight");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Destinations",
                table: "Destinations");

            migrationBuilder.RenameTable(
                name: "Destinations",
                newName: "Destination");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Destination",
                table: "Destination",
                column: "DestinationID");

            migrationBuilder.AddForeignKey(
                name: "FK_Flight_Destination_DestinationID",
                table: "Flight",
                column: "DestinationID",
                principalTable: "Destination",
                principalColumn: "DestinationID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
