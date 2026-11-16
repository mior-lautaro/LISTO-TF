using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LISTO.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class IncorporarPlanEstudios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "plan_estudios",
                columns: table => new
                {
                    plan_estudios_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    esta_activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plan_estudios", x => x.plan_estudios_id);
                    table.CheckConstraint("ck_plan_estudios_codigo_no_vacio", "btrim(codigo) <> ''");
                    table.CheckConstraint("ck_plan_estudios_nombre_no_vacio", "btrim(nombre) <> ''");
                });

            migrationBuilder.CreateTable(
                name: "plan_estudios_version",
                columns: table => new
                {
                    plan_estudios_version_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    plan_estudios_id = table.Column<long>(type: "bigint", nullable: false),
                    numero_version = table.Column<int>(type: "integer", nullable: false),
                    fecha_vigencia_desde = table.Column<DateOnly>(type: "date", nullable: true),
                    fecha_vigencia_hasta = table.Column<DateOnly>(type: "date", nullable: true),
                    estado = table.Column<int>(type: "integer", nullable: false),
                    observaciones = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plan_estudios_version", x => x.plan_estudios_version_id);
                    table.CheckConstraint("ck_plan_estudios_version_estado_valido", "estado IN (1, 2, 3)");
                    table.CheckConstraint("ck_plan_estudios_version_fechas", "fecha_vigencia_hasta IS NULL OR (fecha_vigencia_desde IS NOT NULL AND fecha_vigencia_hasta >= fecha_vigencia_desde)");
                    table.CheckConstraint("ck_plan_estudios_version_numero_positivo", "numero_version > 0");
                    table.CheckConstraint("ck_plan_estudios_version_vigente_con_fecha", "estado <> 2 OR fecha_vigencia_desde IS NOT NULL");
                    table.ForeignKey(
                        name: "FK_plan_estudios_version_plan_estudios_plan_estudios_id",
                        column: x => x.plan_estudios_id,
                        principalTable: "plan_estudios",
                        principalColumn: "plan_estudios_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "plan_curso",
                columns: table => new
                {
                    plan_curso_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    plan_estudios_version_id = table.Column<long>(type: "bigint", nullable: false),
                    curso_id = table.Column<long>(type: "bigint", nullable: false),
                    anio_nivel = table.Column<int>(type: "integer", nullable: false),
                    periodo_recomendado = table.Column<int>(type: "integer", nullable: false),
                    carga_horaria = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plan_curso", x => x.plan_curso_id);
                    table.CheckConstraint("ck_plan_curso_anio_nivel_positivo", "anio_nivel > 0");
                    table.CheckConstraint("ck_plan_curso_carga_horaria_positiva", "carga_horaria > 0");
                    table.CheckConstraint("ck_plan_curso_periodo_valido", "periodo_recomendado IN (1, 2, 3)");
                    table.ForeignKey(
                        name: "FK_plan_curso_curso_curso_id",
                        column: x => x.curso_id,
                        principalTable: "curso",
                        principalColumn: "curso_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_plan_curso_plan_estudios_version_plan_estudios_version_id",
                        column: x => x.plan_estudios_version_id,
                        principalTable: "plan_estudios_version",
                        principalColumn: "plan_estudios_version_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_plan_curso_curso_id",
                table: "plan_curso",
                column: "curso_id");

            migrationBuilder.CreateIndex(
                name: "ux_plan_curso_version_curso",
                table: "plan_curso",
                columns: new[] { "plan_estudios_version_id", "curso_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_plan_estudios_codigo",
                table: "plan_estudios",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_plan_estudios_version_numero",
                table: "plan_estudios_version",
                columns: new[] { "plan_estudios_id", "numero_version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_plan_estudios_version_vigente",
                table: "plan_estudios_version",
                column: "plan_estudios_id",
                unique: true,
                filter: "estado = 2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "plan_curso");

            migrationBuilder.DropTable(
                name: "plan_estudios_version");

            migrationBuilder.DropTable(
                name: "plan_estudios");
        }
    }
}
