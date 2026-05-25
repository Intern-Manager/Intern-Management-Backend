using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InternManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToInternProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "InternProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_InternProfiles_UserId",
                table: "InternProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InternProfiles_Users_UserId",
                table: "InternProfiles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InternProfiles_Users_UserId",
                table: "InternProfiles");

            migrationBuilder.DropIndex(
                name: "IX_InternProfiles_UserId",
                table: "InternProfiles");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "InternProfiles");
        }
    }
}
