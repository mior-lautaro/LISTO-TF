using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LISTO.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class IncorporarEjercicio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ejercicio",
                columns: table => new
                {
                    ejercicio_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    curso_id = table.Column<long>(type: "bigint", nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    estado = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ejercicio", x => x.ejercicio_id);
                    table.CheckConstraint("ck_ejercicio_estado_valido", "estado IN (1, 2)");
                    table.ForeignKey(
                        name: "FK_ejercicio_curso_curso_id",
                        column: x => x.curso_id,
                        principalTable: "curso",
                        principalColumn: "curso_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_ejercicio_curso_id",
                table: "ejercicio",
                column: "curso_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ejercicio");
        }
    }
}
