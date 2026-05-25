using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InternManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGoogleTokensToInternProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GoogleRefreshToken",
                table: "InternProfiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "GoogleTokenExpiry",
                table: "InternProfiles",
                type: "datetime2",
                nullable: true);

            // AuditLogs table already exists, skip creation
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // AuditLogs table already existed, don't drop it
            migrationBuilder.DropColumn(
                name: "GoogleRefreshToken",
                table: "InternProfiles");

            migrationBuilder.DropColumn(
                name: "GoogleTokenExpiry",
                table: "InternProfiles");
        }
    }
}
