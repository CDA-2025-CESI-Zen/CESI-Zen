using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CesiZen.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _220 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AuthFailedAttemptCount",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsSuspended",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthFailedAttemptCount",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsSuspended",
                table: "Users");
        }
    }
}
