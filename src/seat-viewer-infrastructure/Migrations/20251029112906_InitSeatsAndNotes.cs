using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace seat_viewer_infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitSeatsAndNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Seats",
                columns: table => new
                {
                    AircraftModel = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SeatNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Position = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    HasWindow = table.Column<bool>(type: "bit", nullable: false),
                    PowerAvailable = table.Column<bool>(type: "bit", nullable: false),
                    PowerType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    HasInSeatScreen = table.Column<bool>(type: "bit", nullable: false),
                    ExperienceSummary = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seats", x => new { x.AircraftModel, x.SeatNumber });
                });

            migrationBuilder.CreateTable(
                name: "SeatNotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AircraftModel = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SeatNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Text = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeatNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeatNotes_Seats_AircraftModel_SeatNumber",
                        columns: x => new { x.AircraftModel, x.SeatNumber },
                        principalTable: "Seats",
                        principalColumns: new[] { "AircraftModel", "SeatNumber" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SeatNotes_AircraftModel_SeatNumber_UpdatedAt",
                table: "SeatNotes",
                columns: new[] { "AircraftModel", "SeatNumber", "UpdatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Seats_Position",
                table: "Seats",
                column: "Position");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SeatNotes");

            migrationBuilder.DropTable(
                name: "Seats");
        }
    }
}
