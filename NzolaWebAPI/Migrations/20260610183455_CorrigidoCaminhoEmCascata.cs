using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NzolaWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class CorrigidoCaminhoEmCascata : Migration
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

            migrationBuilder.AddColumn<DateTime>(
                name: "DataAtualizacaoPublicacao",
                table: "tb_Publicacao",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "tb_Baze",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "EstadoBaze",
                table: "tb_Baze",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UtilizadorId1",
                table: "tb_Baze",
                type: "int",
                nullable: true);

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
                columns: new[] { "UtilizadorId", "PublicacaoId" });

            migrationBuilder.CreateTable(
                name: "Utilizadores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Genero = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                name: "IX_tb_Comentario_PublicacaoId",
                table: "tb_Comentario",
                column: "PublicacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_Comentario_UtilizadorId",
                table: "tb_Comentario",
                column: "UtilizadorId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_Baze_PublicacaoId_UtilizadorId",
                table: "tb_Baze",
                columns: new[] { "PublicacaoId", "UtilizadorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tb_Baze_UtilizadorId1",
                table: "tb_Baze",
                column: "UtilizadorId1");

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
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tb_Baze_Utilizadores_UtilizadorId1",
                table: "tb_Baze",
                column: "UtilizadorId1",
                principalTable: "Utilizadores",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_tb_Baze_tb_Publicacao_PublicacaoId",
                table: "tb_Baze",
                column: "PublicacaoId",
                principalTable: "tb_Publicacao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tb_Comentario_Utilizadores_UtilizadorId",
                table: "tb_Comentario",
                column: "UtilizadorId",
                principalTable: "Utilizadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tb_Comentario_tb_Publicacao_PublicacaoId",
                table: "tb_Comentario",
                column: "PublicacaoId",
                principalTable: "tb_Publicacao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tb_ConteudoPublicacao_tb_Publicacao_PublicacaoId",
                table: "tb_ConteudoPublicacao",
                column: "PublicacaoId",
                principalTable: "tb_Publicacao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tb_Publicacao_Utilizadores_AutorId",
                table: "tb_Publicacao",
                column: "AutorId",
                principalTable: "Utilizadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tb_Baze_Utilizadores_UtilizadorId",
                table: "tb_Baze");

            migrationBuilder.DropForeignKey(
                name: "FK_tb_Baze_Utilizadores_UtilizadorId1",
                table: "tb_Baze");

            migrationBuilder.DropForeignKey(
                name: "FK_tb_Baze_tb_Publicacao_PublicacaoId",
                table: "tb_Baze");

            migrationBuilder.DropForeignKey(
                name: "FK_tb_Comentario_Utilizadores_UtilizadorId",
                table: "tb_Comentario");

            migrationBuilder.DropForeignKey(
                name: "FK_tb_Comentario_tb_Publicacao_PublicacaoId",
                table: "tb_Comentario");

            migrationBuilder.DropForeignKey(
                name: "FK_tb_ConteudoPublicacao_tb_Publicacao_PublicacaoId",
                table: "tb_ConteudoPublicacao");

            migrationBuilder.DropForeignKey(
                name: "FK_tb_Publicacao_Utilizadores_AutorId",
                table: "tb_Publicacao");

            migrationBuilder.DropTable(
                name: "Seguidores");

            migrationBuilder.DropTable(
                name: "Utilizadores");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tb_Publicacao",
                table: "tb_Publicacao");

            migrationBuilder.DropIndex(
                name: "IX_tb_Publicacao_AutorId",
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

            migrationBuilder.DropIndex(
                name: "IX_tb_Comentario_UtilizadorId",
                table: "tb_Comentario");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tb_Baze",
                table: "tb_Baze");

            migrationBuilder.DropIndex(
                name: "IX_tb_Baze_PublicacaoId_UtilizadorId",
                table: "tb_Baze");

            migrationBuilder.DropIndex(
                name: "IX_tb_Baze_UtilizadorId1",
                table: "tb_Baze");

            migrationBuilder.DropColumn(
                name: "DataAtualizacaoPublicacao",
                table: "tb_Publicacao");

            migrationBuilder.DropColumn(
                name: "EstadoBaze",
                table: "tb_Baze");

            migrationBuilder.DropColumn(
                name: "UtilizadorId1",
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

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Bazes",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

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
