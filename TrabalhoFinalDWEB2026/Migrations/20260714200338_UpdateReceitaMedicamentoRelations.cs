using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrabalhoFinalDWEB2026.Migrations
{
    /// <inheritdoc />
    public partial class UpdateReceitaMedicamentoRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Receitas_MyUsers_MyUserId",
                table: "Receitas");

            migrationBuilder.DropTable(
                name: "MedicamentoReceita");

            migrationBuilder.DropColumn(
                name: "Instructions",
                table: "Receitas");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Medicamentos");

            migrationBuilder.AddColumn<int>(
                name: "DoctorId",
                table: "Receitas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Weight",
                table: "Medicamentos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ReceitaMedicamentos",
                columns: table => new
                {
                    ReceitaId = table.Column<int>(type: "int", nullable: false),
                    MedicamentoId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceitaMedicamentos", x => new { x.ReceitaId, x.MedicamentoId });
                    table.ForeignKey(
                        name: "FK_ReceitaMedicamentos_Medicamentos_MedicamentoId",
                        column: x => x.MedicamentoId,
                        principalTable: "Medicamentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReceitaMedicamentos_Receitas_ReceitaId",
                        column: x => x.ReceitaId,
                        principalTable: "Receitas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Receitas_DoctorId",
                table: "Receitas",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceitaMedicamentos_MedicamentoId",
                table: "ReceitaMedicamentos",
                column: "MedicamentoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Receitas_MyUsers_DoctorId",
                table: "Receitas",
                column: "DoctorId",
                principalTable: "MyUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Receitas_MyUsers_MyUserId",
                table: "Receitas",
                column: "MyUserId",
                principalTable: "MyUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Receitas_MyUsers_DoctorId",
                table: "Receitas");

            migrationBuilder.DropForeignKey(
                name: "FK_Receitas_MyUsers_MyUserId",
                table: "Receitas");

            migrationBuilder.DropTable(
                name: "ReceitaMedicamentos");

            migrationBuilder.DropIndex(
                name: "IX_Receitas_DoctorId",
                table: "Receitas");

            migrationBuilder.DropColumn(
                name: "DoctorId",
                table: "Receitas");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "Medicamentos");

            migrationBuilder.AddColumn<string>(
                name: "Instructions",
                table: "Receitas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Medicamentos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Receitas_MyUsers_MyUserId",
                table: "Receitas",
                column: "MyUserId",
                principalTable: "MyUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
