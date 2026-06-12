using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NzolaWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddRestricoesNomeUtilizadorEGenero : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Genero",
                table: "tb_Utilizadores",
                type: "nvarchar(10)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "tb_Utilizadores",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "NomeUtilizador",
                table: "tb_Utilizadores",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_tb_Utilizadores_NomeUtilizador",
                table: "tb_Utilizadores",
                column: "NomeUtilizador",
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Utilizadores_Genero",
                table: "tb_Utilizadores",
                sql: "Genero IN ('Masculino','Feminino')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_tb_Utilizadores_NomeUtilizador",
                table: "tb_Utilizadores");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Utilizadores_Genero",
                table: "tb_Utilizadores");

            migrationBuilder.DropColumn(
                name: "NomeUtilizador",
                table: "tb_Utilizadores");

            migrationBuilder.AlterColumn<string>(
                name: "Genero",
                table: "tb_Utilizadores",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "tb_Utilizadores",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);
        }
    }
}
