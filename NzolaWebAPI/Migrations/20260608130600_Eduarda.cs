using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NzolaWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class Eduarda : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataAtualizacaoPublicacao",
                table: "tb_Publicacao",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "EstadoBaze",
                table: "tb_Baze",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Utilizadores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    genero = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NomeCompleto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PalavraPasse = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NivelAcesso = table.Column<int>(type: "int", nullable: false),
                    FotoPerfil = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    Biografia = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Privacidade = table.Column<int>(type: "int", nullable: false),
                    EstadoConta = table.Column<int>(type: "int", nullable: false),
                    DataRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataNascimento = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilizadores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Seguidores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SeguidorId = table.Column<int>(type: "int", nullable: false),
                    SeguidoId = table.Column<int>(type: "int", nullable: false),
                    DataInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UtilizadorId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seguidores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Seguidores_Utilizadores_UtilizadorId",
                        column: x => x.UtilizadorId,
                        principalTable: "Utilizadores",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_tb_Publicacao_AutorId",
                table: "tb_Publicacao",
                column: "AutorId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_Comentario_UtilizadorId",
                table: "tb_Comentario",
                column: "UtilizadorId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_Baze_UtilizadorId",
                table: "tb_Baze",
                column: "UtilizadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Seguidores_UtilizadorId",
                table: "Seguidores",
                column: "UtilizadorId");

            migrationBuilder.AddForeignKey(
                name: "FK_tb_Baze_Utilizadores_UtilizadorId",
                table: "tb_Baze",
                column: "UtilizadorId",
                principalTable: "Utilizadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tb_Comentario_Utilizadores_UtilizadorId",
                table: "tb_Comentario",
                column: "UtilizadorId",
                principalTable: "Utilizadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tb_Publicacao_Utilizadores_AutorId",
                table: "tb_Publicacao",
                column: "AutorId",
                principalTable: "Utilizadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tb_Baze_Utilizadores_UtilizadorId",
                table: "tb_Baze");

            migrationBuilder.DropForeignKey(
                name: "FK_tb_Comentario_Utilizadores_UtilizadorId",
                table: "tb_Comentario");

            migrationBuilder.DropForeignKey(
                name: "FK_tb_Publicacao_Utilizadores_AutorId",
                table: "tb_Publicacao");

            migrationBuilder.DropTable(
                name: "Seguidores");

            migrationBuilder.DropTable(
                name: "Utilizadores");

            migrationBuilder.DropIndex(
                name: "IX_tb_Publicacao_AutorId",
                table: "tb_Publicacao");

            migrationBuilder.DropIndex(
                name: "IX_tb_Comentario_UtilizadorId",
                table: "tb_Comentario");

            migrationBuilder.DropIndex(
                name: "IX_tb_Baze_UtilizadorId",
                table: "tb_Baze");

            migrationBuilder.DropColumn(
                name: "DataAtualizacaoPublicacao",
                table: "tb_Publicacao");

            migrationBuilder.DropColumn(
                name: "EstadoBaze",
                table: "tb_Baze");
        }
    }
}
