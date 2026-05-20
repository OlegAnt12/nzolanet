using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NzolaWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class MudancaNoContexto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConteudosPublicacao_Publicacoes_PublicacaoId",
                table: "ConteudosPublicacao");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Publicacoes",
                table: "Publicacoes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ConteudosPublicacao",
                table: "ConteudosPublicacao");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comentarios",
                table: "Comentarios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Bazes",
                table: "Bazes");

            migrationBuilder.RenameTable(
                name: "Publicacoes",
                newName: "tb_Publicacao");

            migrationBuilder.RenameTable(
                name: "ConteudosPublicacao",
                newName: "tb_ConteudoPublicacao");

            migrationBuilder.RenameTable(
                name: "Comentarios",
                newName: "tb_Comentario");

            migrationBuilder.RenameTable(
                name: "Bazes",
                newName: "tb_Baze");

            migrationBuilder.RenameIndex(
                name: "IX_ConteudosPublicacao_PublicacaoId",
                table: "tb_ConteudoPublicacao",
                newName: "IX_tb_ConteudoPublicacao_PublicacaoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tb_Publicacao",
                table: "tb_Publicacao",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tb_ConteudoPublicacao",
                table: "tb_ConteudoPublicacao",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tb_Comentario",
                table: "tb_Comentario",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tb_Baze",
                table: "tb_Baze",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_tb_Comentario_PublicacaoId",
                table: "tb_Comentario",
                column: "PublicacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_Baze_PublicacaoId_UtilizadorId",
                table: "tb_Baze",
                columns: new[] { "PublicacaoId", "UtilizadorId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_tb_Baze_tb_Publicacao_PublicacaoId",
                table: "tb_Baze",
                column: "PublicacaoId",
                principalTable: "tb_Publicacao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tb_Comentario_tb_Publicacao_PublicacaoId",
                table: "tb_Comentario",
                column: "PublicacaoId",
                principalTable: "tb_Publicacao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tb_ConteudoPublicacao_tb_Publicacao_PublicacaoId",
                table: "tb_ConteudoPublicacao",
                column: "PublicacaoId",
                principalTable: "tb_Publicacao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tb_Baze_tb_Publicacao_PublicacaoId",
                table: "tb_Baze");

            migrationBuilder.DropForeignKey(
                name: "FK_tb_Comentario_tb_Publicacao_PublicacaoId",
                table: "tb_Comentario");

            migrationBuilder.DropForeignKey(
                name: "FK_tb_ConteudoPublicacao_tb_Publicacao_PublicacaoId",
                table: "tb_ConteudoPublicacao");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tb_Publicacao",
                table: "tb_Publicacao");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tb_ConteudoPublicacao",
                table: "tb_ConteudoPublicacao");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tb_Comentario",
                table: "tb_Comentario");

            migrationBuilder.DropIndex(
                name: "IX_tb_Comentario_PublicacaoId",
                table: "tb_Comentario");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tb_Baze",
                table: "tb_Baze");

            migrationBuilder.DropIndex(
                name: "IX_tb_Baze_PublicacaoId_UtilizadorId",
                table: "tb_Baze");

            migrationBuilder.RenameTable(
                name: "tb_Publicacao",
                newName: "Publicacoes");

            migrationBuilder.RenameTable(
                name: "tb_ConteudoPublicacao",
                newName: "ConteudosPublicacao");

            migrationBuilder.RenameTable(
                name: "tb_Comentario",
                newName: "Comentarios");

            migrationBuilder.RenameTable(
                name: "tb_Baze",
                newName: "Bazes");

            migrationBuilder.RenameIndex(
                name: "IX_tb_ConteudoPublicacao_PublicacaoId",
                table: "ConteudosPublicacao",
                newName: "IX_ConteudosPublicacao_PublicacaoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Publicacoes",
                table: "Publicacoes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ConteudosPublicacao",
                table: "ConteudosPublicacao",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comentarios",
                table: "Comentarios",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Bazes",
                table: "Bazes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ConteudosPublicacao_Publicacoes_PublicacaoId",
                table: "ConteudosPublicacao",
                column: "PublicacaoId",
                principalTable: "Publicacoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
