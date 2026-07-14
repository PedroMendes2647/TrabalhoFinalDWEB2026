using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrabalhoFinalDWEB2026.Migrations
{
    /// <inheritdoc />
    public partial class ReceitaMedicamentoManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Receitas_Medicamentos_MedicamentoId",
                table: "Receitas");

            migrationBuilder.DropIndex(
                name: "IX_Receitas_MedicamentoId",
                table: "Receitas");

            migrationBuilder.DropColumn(
                name: "MedicamentoId",
                table: "Receitas");

            migrationBuilder.CreateTable(
                name: "MedicamentoReceita",
                columns: table => new
                {
                    MedicamentosId = table.Column<int>(type: "int", nullable: false),
                    ReceitasId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicamentoReceita", x => new { x.MedicamentosId, x.ReceitasId });
                    table.ForeignKey(
                        name: "FK_MedicamentoReceita_Medicamentos_MedicamentosId",
                        column: x => x.MedicamentosId,
                        principalTable: "Medicamentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MedicamentoReceita_Receitas_ReceitasId",
                        column: x => x.ReceitasId,
                        principalTable: "Receitas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MedicamentoReceita_ReceitasId",
                table: "MedicamentoReceita",
                column: "ReceitasId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MedicamentoReceita");

            migrationBuilder.AddColumn<int>(
                name: "MedicamentoId",
                table: "Receitas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Receitas_MedicamentoId",
                table: "Receitas",
                column: "MedicamentoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Receitas_Medicamentos_MedicamentoId",
                table: "Receitas",
                column: "MedicamentoId",
                principalTable: "Medicamentos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
