using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthCare.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EditLabWorkingdaysReviewsTarget : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorkingDays",
                table: "Labs");

            migrationBuilder.AddColumn<Guid>(
                name: "TargetId",
                table: "Reviews",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "TargetType",
                table: "Reviews",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "WorkingDays_IsFridayOpen",
                table: "Labs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WorkingDays_IsMondayOpen",
                table: "Labs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WorkingDays_IsSaturdayOpen",
                table: "Labs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WorkingDays_IsSundayOpen",
                table: "Labs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WorkingDays_IsThursdayOpen",
                table: "Labs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WorkingDays_IsTuesdayOpen",
                table: "Labs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WorkingDays_IsWednesdayOpen",
                table: "Labs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TargetId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "TargetType",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "WorkingDays_IsFridayOpen",
                table: "Labs");

            migrationBuilder.DropColumn(
                name: "WorkingDays_IsMondayOpen",
                table: "Labs");

            migrationBuilder.DropColumn(
                name: "WorkingDays_IsSaturdayOpen",
                table: "Labs");

            migrationBuilder.DropColumn(
                name: "WorkingDays_IsSundayOpen",
                table: "Labs");

            migrationBuilder.DropColumn(
                name: "WorkingDays_IsThursdayOpen",
                table: "Labs");

            migrationBuilder.DropColumn(
                name: "WorkingDays_IsTuesdayOpen",
                table: "Labs");

            migrationBuilder.DropColumn(
                name: "WorkingDays_IsWednesdayOpen",
                table: "Labs");

            migrationBuilder.AddColumn<string>(
                name: "WorkingDays",
                table: "Labs",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }
    }
}
