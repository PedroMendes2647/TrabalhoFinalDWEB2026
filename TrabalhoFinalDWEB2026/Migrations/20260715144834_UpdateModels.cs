using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrabalhoFinalDWEB2026.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReceitaMedicamentos_Medicamentos_MedicamentoId",
                table: "ReceitaMedicamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Receitas_AspNetUsers_FarmaceutaId",
                table: "Receitas");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceitaMedicamentos_Medicamentos_MedicamentoId",
                table: "ReceitaMedicamentos",
                column: "MedicamentoId",
                principalTable: "Medicamentos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Receitas_AspNetUsers_FarmaceutaId",
                table: "Receitas",
                column: "FarmaceutaId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReceitaMedicamentos_Medicamentos_MedicamentoId",
                table: "ReceitaMedicamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Receitas_AspNetUsers_FarmaceutaId",
                table: "Receitas");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceitaMedicamentos_Medicamentos_MedicamentoId",
                table: "ReceitaMedicamentos",
                column: "MedicamentoId",
                principalTable: "Medicamentos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Receitas_AspNetUsers_FarmaceutaId",
                table: "Receitas",
                column: "FarmaceutaId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
