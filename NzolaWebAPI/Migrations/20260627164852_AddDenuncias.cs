using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NzolaWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddDenuncias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ConcordaComTermos",
                table: "tb_Utilizadores",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "tb_Denuncias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoEntidade = table.Column<int>(type: "int", nullable: false),
                    IdEntidade = table.Column<int>(type: "int", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DenuncianteId = table.Column<int>(type: "int", nullable: false),
                    DataDenuncia = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstadoDenuncia = table.Column<string>(type: "nvarchar(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_Denuncias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tb_Denuncias_tb_Utilizadores_DenuncianteId",
                        column: x => x.DenuncianteId,
                        principalTable: "tb_Utilizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tb_Denuncias_DenuncianteId",
                table: "tb_Denuncias",
                column: "DenuncianteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tb_Denuncias");

            migrationBuilder.DropColumn(
                name: "ConcordaComTermos",
                table: "tb_Utilizadores");
        }
    }
}
