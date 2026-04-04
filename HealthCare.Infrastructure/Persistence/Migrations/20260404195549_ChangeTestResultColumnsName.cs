using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthCare.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTestResultColumnsName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AttachmentUrl",
                table: "TestResults",
                newName: "ResultFileUrl");

            migrationBuilder.RenameColumn(
                name: "AttachmentPublicId",
                table: "TestResults",
                newName: "ResultFilePublicId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ResultFileUrl",
                table: "TestResults",
                newName: "AttachmentUrl");

            migrationBuilder.RenameColumn(
                name: "ResultFilePublicId",
                table: "TestResults",
                newName: "AttachmentPublicId");
        }
    }
}
