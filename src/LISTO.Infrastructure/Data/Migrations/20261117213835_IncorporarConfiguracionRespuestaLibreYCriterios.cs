using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LISTO.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class IncorporarConfiguracionRespuestaLibreYCriterios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "configuracion_respuesta_libre",
                columns: table => new
                {
                    pregunta_id = table.Column<long>(type: "bigint", nullable: false),
                    respuesta_referencia = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_configuracion_respuesta_libre", x => x.pregunta_id);
                    table.CheckConstraint("ck_configuracion_respuesta_libre_referencia_no_vacia", "length(trim(respuesta_referencia)) > 0");
                    table.ForeignKey(
                        name: "FK_configuracion_respuesta_libre_pregunta_pregunta_id",
                        column: x => x.pregunta_id,
                        principalTable: "pregunta",
                        principalColumn: "pregunta_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "criterio_evaluacion",
                columns: table => new
                {
                    criterio_evaluacion_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    pregunta_id = table.Column<long>(type: "bigint", nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: false),
                    ponderacion = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    orden = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_criterio_evaluacion", x => x.criterio_evaluacion_id);
                    table.CheckConstraint("ck_criterio_evaluacion_descripcion_no_vacia", "length(trim(descripcion)) > 0");
                    table.CheckConstraint("ck_criterio_evaluacion_orden", "orden > 0");
                    table.CheckConstraint("ck_criterio_evaluacion_ponderacion", "ponderacion > 0 AND ponderacion <= 100");
                    table.ForeignKey(
                        name: "FK_criterio_evaluacion_configuracion_respuesta_libre_pregunta_~",
                        column: x => x.pregunta_id,
                        principalTable: "configuracion_respuesta_libre",
                        principalColumn: "pregunta_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ux_criterio_evaluacion_pregunta_orden",
                table: "criterio_evaluacion",
                columns: new[] { "pregunta_id", "orden" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "criterio_evaluacion");

            migrationBuilder.DropTable(
                name: "configuracion_respuesta_libre");
        }
    }
}
