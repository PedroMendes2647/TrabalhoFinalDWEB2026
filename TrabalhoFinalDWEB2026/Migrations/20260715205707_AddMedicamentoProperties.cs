using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrabalhoFinalDWEB2026.Migrations
{
    /// <inheritdoc />
    public partial class AddMedicamentoProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataDispensacao",
                table: "Receitas",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Dosagem",
                table: "Medicamentos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tipo",
                table: "Medicamentos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataDispensacao",
                table: "Receitas");

            migrationBuilder.DropColumn(
                name: "Dosagem",
                table: "Medicamentos");

            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "Medicamentos");
        }
    }
}
