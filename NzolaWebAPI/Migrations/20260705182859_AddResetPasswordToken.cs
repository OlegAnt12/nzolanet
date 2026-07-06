using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NzolaWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddResetPasswordToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ResetTokenExpiraEm",
                table: "tb_Utilizadores",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResetTokenRedefinirPassword",
                table: "tb_Utilizadores",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResetTokenExpiraEm",
                table: "tb_Utilizadores");

            migrationBuilder.DropColumn(
                name: "ResetTokenRedefinirPassword",
                table: "tb_Utilizadores");
        }
    }
}
