using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LISTO.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class IncorporarConfiguracionVerdaderoFalso : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "configuracion_verdadero_falso",
                columns: table => new
                {
                    pregunta_id = table.Column<long>(type: "bigint", nullable: false),
                    respuesta_correcta = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_configuracion_verdadero_falso", x => x.pregunta_id);
                    table.ForeignKey(
                        name: "FK_configuracion_verdadero_falso_pregunta_pregunta_id",
                        column: x => x.pregunta_id,
                        principalTable: "pregunta",
                        principalColumn: "pregunta_id",
                        onDelete: ReferentialAction.Restrict);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "configuracion_verdadero_falso");
        }
    }
}
