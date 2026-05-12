using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InternManagement.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedDefaultRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleName", "Description" },
                values: new object[,]
                {
                    { "Admin", "System administrator with full access" },
                    { "HR Manager", "Manages recruitment and intern profiles" },
                    { "Coordinator", "Coordinates training programs and schedules" },
                    { "Mentor", "Mentors and evaluates interns" },
                    { "Intern", "Internship participant" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleName",
                keyValue: "Admin");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleName",
                keyValue: "HR Manager");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleName",
                keyValue: "Coordinator");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleName",
                keyValue: "Mentor");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleName",
                keyValue: "Intern");
        }
    }
}
