using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NzolaWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class PublicacaoRefactorado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tb_FicheirosConteudo_tb_ConteudosPublicacao_ConteudoPublicacaoId",
                table: "tb_FicheirosConteudo");

            migrationBuilder.DropTable(
                name: "tb_ConteudosPublicacao");

            migrationBuilder.RenameColumn(
                name: "ConteudoPublicacaoId",
                table: "tb_FicheirosConteudo",
                newName: "PublicacaoId");

            migrationBuilder.RenameIndex(
                name: "IX_tb_FicheirosConteudo_ConteudoPublicacaoId",
                table: "tb_FicheirosConteudo",
                newName: "IX_tb_FicheirosConteudo_PublicacaoId");

            migrationBuilder.AddColumn<string>(
                name: "Texto",
                table: "tb_Publicacoes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_tb_FicheirosConteudo_tb_Publicacoes_PublicacaoId",
                table: "tb_FicheirosConteudo",
                column: "PublicacaoId",
                principalTable: "tb_Publicacoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tb_FicheirosConteudo_tb_Publicacoes_PublicacaoId",
                table: "tb_FicheirosConteudo");

            migrationBuilder.DropColumn(
                name: "Texto",
                table: "tb_Publicacoes");

            migrationBuilder.RenameColumn(
                name: "PublicacaoId",
                table: "tb_FicheirosConteudo",
                newName: "ConteudoPublicacaoId");

            migrationBuilder.RenameIndex(
                name: "IX_tb_FicheirosConteudo_PublicacaoId",
                table: "tb_FicheirosConteudo",
                newName: "IX_tb_FicheirosConteudo_ConteudoPublicacaoId");

            migrationBuilder.CreateTable(
                name: "tb_ConteudosPublicacao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PublicacaoId = table.Column<int>(type: "int", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Texto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoConteudo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_ConteudosPublicacao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tb_ConteudosPublicacao_tb_Publicacoes_PublicacaoId",
                        column: x => x.PublicacaoId,
                        principalTable: "tb_Publicacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tb_ConteudosPublicacao_PublicacaoId",
                table: "tb_ConteudosPublicacao",
                column: "PublicacaoId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_tb_FicheirosConteudo_tb_ConteudosPublicacao_ConteudoPublicacaoId",
                table: "tb_FicheirosConteudo",
                column: "ConteudoPublicacaoId",
                principalTable: "tb_ConteudosPublicacao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
