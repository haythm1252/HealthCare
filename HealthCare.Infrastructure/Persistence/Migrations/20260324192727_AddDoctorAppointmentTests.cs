using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthCare.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDoctorAppointmentTests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequiredTests",
                table: "DoctorAppointments");

            migrationBuilder.CreateTable(
                name: "DoctorAppointmentTests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DoctorAppointmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorAppointmentTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DoctorAppointmentTests_DoctorAppointments_DoctorAppointmentId",
                        column: x => x.DoctorAppointmentId,
                        principalTable: "DoctorAppointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DoctorAppointmentTests_Tests_TestId",
                        column: x => x.TestId,
                        principalTable: "Tests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DoctorAppointmentTests_DoctorAppointmentId",
                table: "DoctorAppointmentTests",
                column: "DoctorAppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorAppointmentTests_TestId",
                table: "DoctorAppointmentTests",
                column: "TestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DoctorAppointmentTests");

            migrationBuilder.AddColumn<string>(
                name: "RequiredTests",
                table: "DoctorAppointments",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
