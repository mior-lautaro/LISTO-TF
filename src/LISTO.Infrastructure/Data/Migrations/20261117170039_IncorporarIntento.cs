using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LISTO.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class IncorporarIntento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "intento",
                columns: table => new
                {
                    intento_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    estudiante_id = table.Column<long>(type: "bigint", nullable: false),
                    ejercicio_id = table.Column<long>(type: "bigint", nullable: false),
                    ejercicio_version_id = table.Column<long>(type: "bigint", nullable: false),
                    estado = table.Column<int>(type: "integer", nullable: false),
                    fecha_inicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    fecha_envio = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    fecha_evaluacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_intento", x => x.intento_id);
                    table.CheckConstraint("ck_intento_estado_valido", "estado IN (1, 2, 3)");
                    table.CheckConstraint("ck_intento_fechas_estado", "(estado = 1 AND fecha_envio IS NULL AND fecha_evaluacion IS NULL) OR (estado = 2 AND fecha_envio IS NOT NULL AND fecha_evaluacion IS NULL) OR (estado = 3 AND fecha_envio IS NOT NULL AND fecha_evaluacion IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_intento_ejercicio_ejercicio_id",
                        column: x => x.ejercicio_id,
                        principalTable: "ejercicio",
                        principalColumn: "ejercicio_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_intento_ejercicio_version_ejercicio_version_id",
                        column: x => x.ejercicio_version_id,
                        principalTable: "ejercicio_version",
                        principalColumn: "ejercicio_version_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_intento_estudiante_estudiante_id",
                        column: x => x.estudiante_id,
                        principalTable: "estudiante",
                        principalColumn: "estudiante_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_intento_ejercicio_id",
                table: "intento",
                column: "ejercicio_id");

            migrationBuilder.CreateIndex(
                name: "IX_intento_ejercicio_version_id",
                table: "intento",
                column: "ejercicio_version_id");

            migrationBuilder.CreateIndex(
                name: "ux_intento_estudiante_ejercicio_en_curso",
                table: "intento",
                columns: new[] { "estudiante_id", "ejercicio_id" },
                unique: true,
                filter: "estado = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "intento");
        }
    }
}
