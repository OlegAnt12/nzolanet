using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NzolaWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class MigNotificacoesRemoveMultipleCascadesOrCyclesOnDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tb_Utilizadores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Genero = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    NomeUtilizador = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NomeCompleto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PalavraPasse = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NivelAcesso = table.Column<string>(type: "nvarchar(8)", nullable: false),
                    FotoPerfil = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Biografia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Privacidade = table.Column<string>(type: "nvarchar(8)", nullable: false),
                    EstadoConta = table.Column<string>(type: "nvarchar(8)", nullable: false),
                    DataRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataNascimento = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_Utilizadores", x => x.Id);
                    table.CheckConstraint("CK_Utilizadores_Genero", "Genero IN ('Masculino','Feminino')");
                });

            migrationBuilder.CreateTable(
                name: "tb_Notificacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UtilizadorId = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    OrigemId = table.Column<int>(type: "int", nullable: false),
                    Mensagem = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Lida = table.Column<bool>(type: "bit", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_Notificacoes", x => x.Id);
                    table.CheckConstraint("CK_Notificacoes_Tipo", "Tipo IN ('Baze','Comentario', 'Seguidor')");
                    table.ForeignKey(
                        name: "FK_tb_Notificacoes_tb_Utilizadores_OrigemId",
                        column: x => x.OrigemId,
                        principalTable: "tb_Utilizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tb_Notificacoes_tb_Utilizadores_UtilizadorId",
                        column: x => x.UtilizadorId,
                        principalTable: "tb_Utilizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tb_Publicacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AutorId = table.Column<int>(type: "int", nullable: false),
                    Texto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Existencia = table.Column<string>(type: "nvarchar(11)", nullable: false),
                    QuantidadeBazes = table.Column<int>(type: "int", nullable: false),
                    QuantidadeComentarios = table.Column<int>(type: "int", nullable: false),
                    DataPublicacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAtualizacaoPublicacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_Publicacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tb_Publicacoes_tb_Utilizadores_AutorId",
                        column: x => x.AutorId,
                        principalTable: "tb_Utilizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tb_Seguidores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SeguidorId = table.Column<int>(type: "int", nullable: false),
                    SeguidoId = table.Column<int>(type: "int", nullable: false),
                    DataInicio = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_Seguidores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tb_Seguidores_tb_Utilizadores_SeguidoId",
                        column: x => x.SeguidoId,
                        principalTable: "tb_Utilizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tb_Seguidores_tb_Utilizadores_SeguidorId",
                        column: x => x.SeguidorId,
                        principalTable: "tb_Utilizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tb_Bazes",
                columns: table => new
                {
                    PublicacaoId = table.Column<int>(type: "int", nullable: false),
                    UtilizadorId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    EstadoBaze = table.Column<int>(type: "int", nullable: false),
                    DataInteracao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UtilizadorId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_Bazes", x => new { x.UtilizadorId, x.PublicacaoId });
                    table.ForeignKey(
                        name: "FK_tb_Bazes_tb_Publicacoes_PublicacaoId",
                        column: x => x.PublicacaoId,
                        principalTable: "tb_Publicacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tb_Bazes_tb_Utilizadores_UtilizadorId",
                        column: x => x.UtilizadorId,
                        principalTable: "tb_Utilizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tb_Bazes_tb_Utilizadores_UtilizadorId1",
                        column: x => x.UtilizadorId1,
                        principalTable: "tb_Utilizadores",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "tb_Comentarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PublicacaoId = table.Column<int>(type: "int", nullable: false),
                    ComentadorId = table.Column<int>(type: "int", nullable: false),
                    ConteudoComentario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataComentario = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataActualizacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_Comentarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tb_Comentarios_tb_Publicacoes_PublicacaoId",
                        column: x => x.PublicacaoId,
                        principalTable: "tb_Publicacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tb_Comentarios_tb_Utilizadores_ComentadorId",
                        column: x => x.ComentadorId,
                        principalTable: "tb_Utilizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tb_FicheirosConteudo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PublicacaoId = table.Column<int>(type: "int", nullable: false),
                    CaminhoFicheiro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoMime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TamanhoBytes = table.Column<long>(type: "bigint", nullable: false),
                    DataUpload = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_FicheirosConteudo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tb_FicheirosConteudo_tb_Publicacoes_PublicacaoId",
                        column: x => x.PublicacaoId,
                        principalTable: "tb_Publicacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tb_Bazes_PublicacaoId_UtilizadorId",
                table: "tb_Bazes",
                columns: new[] { "PublicacaoId", "UtilizadorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tb_Bazes_UtilizadorId1",
                table: "tb_Bazes",
                column: "UtilizadorId1");

            migrationBuilder.CreateIndex(
                name: "IX_tb_Comentarios_ComentadorId",
                table: "tb_Comentarios",
                column: "ComentadorId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_Comentarios_PublicacaoId",
                table: "tb_Comentarios",
                column: "PublicacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_FicheirosConteudo_PublicacaoId",
                table: "tb_FicheirosConteudo",
                column: "PublicacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_Notificacoes_OrigemId",
                table: "tb_Notificacoes",
                column: "OrigemId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_Notificacoes_UtilizadorId",
                table: "tb_Notificacoes",
                column: "UtilizadorId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_Publicacoes_AutorId",
                table: "tb_Publicacoes",
                column: "AutorId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_Seguidores_SeguidoId",
                table: "tb_Seguidores",
                column: "SeguidoId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_Seguidores_SeguidorId",
                table: "tb_Seguidores",
                column: "SeguidorId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_Utilizadores_Email",
                table: "tb_Utilizadores",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tb_Utilizadores_NomeUtilizador",
                table: "tb_Utilizadores",
                column: "NomeUtilizador",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tb_Bazes");

            migrationBuilder.DropTable(
                name: "tb_Comentarios");

            migrationBuilder.DropTable(
                name: "tb_FicheirosConteudo");

            migrationBuilder.DropTable(
                name: "tb_Notificacoes");

            migrationBuilder.DropTable(
                name: "tb_Seguidores");

            migrationBuilder.DropTable(
                name: "tb_Publicacoes");

            migrationBuilder.DropTable(
                name: "tb_Utilizadores");
        }
    }
}
