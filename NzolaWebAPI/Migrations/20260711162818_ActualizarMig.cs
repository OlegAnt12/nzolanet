using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NzolaWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class ActualizarMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tb_Bazes_tb_Utilizadores_UtilizadorId1",
                table: "tb_Bazes");

            migrationBuilder.DropIndex(
                name: "IX_tb_Bazes_UtilizadorId1",
                table: "tb_Bazes");

            migrationBuilder.DropColumn(
                name: "UtilizadorId1",
                table: "tb_Bazes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UtilizadorId1",
                table: "tb_Bazes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_tb_Bazes_UtilizadorId1",
                table: "tb_Bazes",
                column: "UtilizadorId1");

            migrationBuilder.AddForeignKey(
                name: "FK_tb_Bazes_tb_Utilizadores_UtilizadorId1",
                table: "tb_Bazes",
                column: "UtilizadorId1",
                principalTable: "tb_Utilizadores",
                principalColumn: "Id");
        }
    }
}
