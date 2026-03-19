using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthCare.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EditSlotAndShifts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DoctorSlots_DoctorId",
                table: "DoctorSlots");

            migrationBuilder.DropColumn(
                name: "DurationInMinutes",
                table: "DoctorSlots");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "DoctorSlots");

            migrationBuilder.AddColumn<bool>(
                name: "IsBooked",
                table: "NurseShifts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateOnly>(
                name: "Date",
                table: "DoctorSlots",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "EndTime",
                table: "DoctorSlots",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "StartTime",
                table: "DoctorSlots",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.CreateIndex(
                name: "IX_DoctorSlots_DoctorId_Date_StartTime",
                table: "DoctorSlots",
                columns: new[] { "DoctorId", "Date", "StartTime" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DoctorSlots_DoctorId_Date_StartTime",
                table: "DoctorSlots");

            migrationBuilder.DropColumn(
                name: "IsBooked",
                table: "NurseShifts");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "DoctorSlots");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "DoctorSlots");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "DoctorSlots");

            migrationBuilder.AddColumn<int>(
                name: "DurationInMinutes",
                table: "DoctorSlots",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "DoctorSlots",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_DoctorSlots_DoctorId",
                table: "DoctorSlots",
                column: "DoctorId");
        }
    }
}
