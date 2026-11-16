using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LISTO.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class IncorporarEstudianteInscripcion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "estudiante",
                columns: table => new
                {
                    estudiante_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usuario_id = table.Column<long>(type: "bigint", nullable: false),
                    dni = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    fecha_alta = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    esta_activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_estudiante", x => x.estudiante_id);
                    table.CheckConstraint("ck_estudiante_dni_valido", "dni ~ '^[0-9]{7,8}$'");
                    table.ForeignKey(
                        name: "FK_estudiante_usuario_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuario",
                        principalColumn: "usuario_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "inscripcion",
                columns: table => new
                {
                    inscripcion_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    estudiante_id = table.Column<long>(type: "bigint", nullable: false),
                    plan_curso_id = table.Column<long>(type: "bigint", nullable: false),
                    fecha_inscripcion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    estado = table.Column<int>(type: "integer", nullable: false),
                    fecha_resolucion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inscripcion", x => x.inscripcion_id);
                    table.CheckConstraint("ck_inscripcion_estado_valido", "estado IN (1, 2, 3, 4, 5)");
                    table.CheckConstraint("ck_inscripcion_fecha_resolucion", "(estado = 1 AND fecha_resolucion IS NULL) OR (estado IN (2, 3, 4, 5) AND fecha_resolucion IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_inscripcion_estudiante_estudiante_id",
                        column: x => x.estudiante_id,
                        principalTable: "estudiante",
                        principalColumn: "estudiante_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_inscripcion_plan_curso_plan_curso_id",
                        column: x => x.plan_curso_id,
                        principalTable: "plan_curso",
                        principalColumn: "plan_curso_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ux_estudiante_dni",
                table: "estudiante",
                column: "dni",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_estudiante_usuario",
                table: "estudiante",
                column: "usuario_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_inscripcion_plan_curso_id",
                table: "inscripcion",
                column: "plan_curso_id");

            migrationBuilder.CreateIndex(
                name: "ux_inscripcion_estudiante_plan_curso_activa",
                table: "inscripcion",
                columns: new[] { "estudiante_id", "plan_curso_id" },
                unique: true,
                filter: "estado = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "inscripcion");

            migrationBuilder.DropTable(
                name: "estudiante");
        }
    }
}
