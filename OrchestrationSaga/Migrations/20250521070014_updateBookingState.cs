using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrchestrationSaga.Migrations
{
    /// <inheritdoc />
    public partial class updateBookingState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentSuccess",
                table: "BookingStates");

            migrationBuilder.DropColumn(
                name: "SeatHoldSuccess",
                table: "BookingStates");

            migrationBuilder.AddColumn<Guid>(
                name: "BookingId",
                table: "BookingStates",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ScreeningId",
                table: "BookingStates",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "SeatIds",
                table: "BookingStates",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookingId",
                table: "BookingStates");

            migrationBuilder.DropColumn(
                name: "ScreeningId",
                table: "BookingStates");

            migrationBuilder.DropColumn(
                name: "SeatIds",
                table: "BookingStates");

            migrationBuilder.AddColumn<bool>(
                name: "PaymentSuccess",
                table: "BookingStates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SeatHoldSuccess",
                table: "BookingStates",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
