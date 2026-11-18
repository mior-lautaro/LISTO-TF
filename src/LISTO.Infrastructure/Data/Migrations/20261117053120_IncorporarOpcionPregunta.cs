using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LISTO.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class IncorporarOpcionPregunta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "opcion_pregunta",
                columns: table => new
                {
                    opcion_pregunta_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    pregunta_id = table.Column<long>(type: "bigint", nullable: false),
                    texto = table.Column<string>(type: "text", nullable: false),
                    orden = table.Column<int>(type: "integer", nullable: false),
                    es_correcta = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_opcion_pregunta", x => x.opcion_pregunta_id);
                    table.CheckConstraint("ck_opcion_pregunta_orden_positivo", "orden > 0");
                    table.ForeignKey(
                        name: "FK_opcion_pregunta_pregunta_pregunta_id",
                        column: x => x.pregunta_id,
                        principalTable: "pregunta",
                        principalColumn: "pregunta_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ux_opcion_pregunta_correcta",
                table: "opcion_pregunta",
                column: "pregunta_id",
                unique: true,
                filter: "es_correcta = TRUE");

            migrationBuilder.CreateIndex(
                name: "ux_opcion_pregunta_pregunta_orden",
                table: "opcion_pregunta",
                columns: new[] { "pregunta_id", "orden" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "opcion_pregunta");
        }
    }
}
