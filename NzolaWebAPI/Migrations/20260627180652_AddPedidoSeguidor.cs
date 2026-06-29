using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NzolaWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddPedidoSeguidor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tb_PedidosSeguir",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SeguidorId = table.Column<int>(type: "int", nullable: false),
                    SeguidoId = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    DataPedido = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_PedidosSeguir", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tb_PedidosSeguir_tb_Utilizadores_SeguidoId",
                        column: x => x.SeguidoId,
                        principalTable: "tb_Utilizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tb_PedidosSeguir_tb_Utilizadores_SeguidorId",
                        column: x => x.SeguidorId,
                        principalTable: "tb_Utilizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tb_PedidosSeguir_SeguidoId",
                table: "tb_PedidosSeguir",
                column: "SeguidoId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_PedidosSeguir_SeguidorId",
                table: "tb_PedidosSeguir",
                column: "SeguidorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tb_PedidosSeguir");
        }
    }
}
