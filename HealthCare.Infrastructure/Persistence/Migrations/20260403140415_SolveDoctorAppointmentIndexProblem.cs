using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthCare.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SolveDoctorAppointmentIndexProblem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DoctorAppointments_DoctorSlotId",
                table: "DoctorAppointments");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorAppointments_DoctorSlotId",
                table: "DoctorAppointments",
                column: "DoctorSlotId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DoctorAppointments_DoctorSlotId",
                table: "DoctorAppointments");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorAppointments_DoctorSlotId",
                table: "DoctorAppointments",
                column: "DoctorSlotId",
                unique: true,
                filter: "[Status] != 'Cancelled'");
        }
    }
}
