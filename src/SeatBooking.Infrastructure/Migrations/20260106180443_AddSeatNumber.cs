using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeatBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSeatNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Number",
                table: "Seats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Seats_Number",
                table: "Seats",
                column: "Number",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Seats_Number",
                table: "Seats");

            migrationBuilder.DropColumn(
                name: "Number",
                table: "Seats");
        }
    }
}
