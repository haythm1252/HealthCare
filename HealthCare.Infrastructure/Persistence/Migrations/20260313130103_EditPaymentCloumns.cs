using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthCare.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EditPaymentCloumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentId",
                table: "DoctorAppointments",
                newName: "PaymentTransactionId");

            migrationBuilder.AddColumn<string>(
                name: "PaymentOrderId",
                table: "DoctorAppointments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "DoctorAppointments",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentOrderId",
                table: "DoctorAppointments");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "DoctorAppointments");

            migrationBuilder.RenameColumn(
                name: "PaymentTransactionId",
                table: "DoctorAppointments",
                newName: "PaymentId");
        }
    }
}
