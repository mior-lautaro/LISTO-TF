using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LISTO.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class IncorporarEjercicioVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ejercicio_version",
                columns: table => new
                {
                    ejercicio_version_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ejercicio_id = table.Column<long>(type: "bigint", nullable: false),
                    numero_version = table.Column<int>(type: "integer", nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    fecha_publicacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    estado = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ejercicio_version", x => x.ejercicio_version_id);
                    table.CheckConstraint("ck_ejercicio_version_estado_valido", "estado IN (1, 2, 3, 4)");
                    table.CheckConstraint("ck_ejercicio_version_numero_positivo", "numero_version > 0");
                    table.ForeignKey(
                        name: "FK_ejercicio_version_ejercicio_ejercicio_id",
                        column: x => x.ejercicio_id,
                        principalTable: "ejercicio",
                        principalColumn: "ejercicio_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ux_ejercicio_version_ejercicio_numero",
                table: "ejercicio_version",
                columns: new[] { "ejercicio_id", "numero_version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_ejercicio_version_publicada",
                table: "ejercicio_version",
                column: "ejercicio_id",
                unique: true,
                filter: "estado = 3");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ejercicio_version");
        }
    }
}
