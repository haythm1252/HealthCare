using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthCare.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EditNurseAppointment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Fee",
                table: "NurseAppointments",
                newName: "TotalFee");

            migrationBuilder.AddColumn<int>(
                name: "Hours",
                table: "NurseAppointments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ServiceType",
                table: "NurseAppointments",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hours",
                table: "NurseAppointments");

            migrationBuilder.DropColumn(
                name: "ServiceType",
                table: "NurseAppointments");

            migrationBuilder.RenameColumn(
                name: "TotalFee",
                table: "NurseAppointments",
                newName: "Fee");
        }
    }
}
