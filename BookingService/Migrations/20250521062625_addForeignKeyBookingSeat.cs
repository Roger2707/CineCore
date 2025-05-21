using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingService.Migrations
{
    /// <inheritdoc />
    public partial class addForeignKeyBookingSeat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_BookingSeats_BookingId",
                table: "BookingSeats",
                column: "BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingSeats_Bookings_BookingId",
                table: "BookingSeats",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingSeats_Bookings_BookingId",
                table: "BookingSeats");

            migrationBuilder.DropIndex(
                name: "IX_BookingSeats_BookingId",
                table: "BookingSeats");
        }
    }
}
