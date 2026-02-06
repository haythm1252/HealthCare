using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthCare.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLabShift : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabAppointments_LabShifts_LabShiftId",
                table: "LabAppointments");

            migrationBuilder.DropTable(
                name: "LabShifts");

            migrationBuilder.DropIndex(
                name: "IX_LabAppointments_LabShiftId",
                table: "LabAppointments");

            migrationBuilder.DropColumn(
                name: "LabShiftId",
                table: "LabAppointments");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "Doctors");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "ClosingTime",
                table: "Labs",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "OpeningTime",
                table: "Labs",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<string>(
                name: "WorkingDays",
                table: "Labs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateOnly>(
                name: "Date",
                table: "LabAppointments",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClosingTime",
                table: "Labs");

            migrationBuilder.DropColumn(
                name: "OpeningTime",
                table: "Labs");

            migrationBuilder.DropColumn(
                name: "WorkingDays",
                table: "Labs");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "LabAppointments");

            migrationBuilder.AddColumn<Guid>(
                name: "LabShiftId",
                table: "LabAppointments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "Doctors",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "LabShifts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LabId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabShifts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LabShifts_Labs_LabId",
                        column: x => x.LabId,
                        principalTable: "Labs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LabAppointments_LabShiftId",
                table: "LabAppointments",
                column: "LabShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_LabShifts_LabId",
                table: "LabShifts",
                column: "LabId");

            migrationBuilder.AddForeignKey(
                name: "FK_LabAppointments_LabShifts_LabShiftId",
                table: "LabAppointments",
                column: "LabShiftId",
                principalTable: "LabShifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
