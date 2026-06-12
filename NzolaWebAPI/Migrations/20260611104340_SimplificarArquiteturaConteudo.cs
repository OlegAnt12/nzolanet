using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NzolaWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class SimplificarArquiteturaConteudo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_tb_ConteudosPublicacao_PublicacaoId",
                table: "tb_ConteudosPublicacao");

            migrationBuilder.DropColumn(
                name: "Ordem",
                table: "tb_ConteudosPublicacao");

            migrationBuilder.RenameColumn(
                name: "Conteudo",
                table: "tb_ConteudosPublicacao",
                newName: "Texto");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataCriacao",
                table: "tb_ConteudosPublicacao",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "tb_FicheirosConteudo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConteudoPublicacaoId = table.Column<int>(type: "int", nullable: false),
                    CaminhoFicheiro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoMime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TamanhoBytes = table.Column<long>(type: "bigint", nullable: false),
                    DataUpload = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_FicheirosConteudo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tb_FicheirosConteudo_tb_ConteudosPublicacao_ConteudoPublicacaoId",
                        column: x => x.ConteudoPublicacaoId,
                        principalTable: "tb_ConteudosPublicacao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tb_ConteudosPublicacao_PublicacaoId",
                table: "tb_ConteudosPublicacao",
                column: "PublicacaoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tb_FicheirosConteudo_ConteudoPublicacaoId",
                table: "tb_FicheirosConteudo",
                column: "ConteudoPublicacaoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tb_FicheirosConteudo");

            migrationBuilder.DropIndex(
                name: "IX_tb_ConteudosPublicacao_PublicacaoId",
                table: "tb_ConteudosPublicacao");

            migrationBuilder.DropColumn(
                name: "DataCriacao",
                table: "tb_ConteudosPublicacao");

            migrationBuilder.RenameColumn(
                name: "Texto",
                table: "tb_ConteudosPublicacao",
                newName: "Conteudo");

            migrationBuilder.AddColumn<int>(
                name: "Ordem",
                table: "tb_ConteudosPublicacao",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_tb_ConteudosPublicacao_PublicacaoId",
                table: "tb_ConteudosPublicacao",
                column: "PublicacaoId");
        }
    }
}
