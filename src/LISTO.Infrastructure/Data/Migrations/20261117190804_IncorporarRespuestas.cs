using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LISTO.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class IncorporarRespuestas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "respuesta",
                columns: table => new
                {
                    respuesta_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    intento_id = table.Column<long>(type: "bigint", nullable: false),
                    pregunta_id = table.Column<long>(type: "bigint", nullable: false),
                    fecha_registro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_respuesta", x => x.respuesta_id);
                    table.ForeignKey(
                        name: "FK_respuesta_intento_intento_id",
                        column: x => x.intento_id,
                        principalTable: "intento",
                        principalColumn: "intento_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_respuesta_pregunta_pregunta_id",
                        column: x => x.pregunta_id,
                        principalTable: "pregunta",
                        principalColumn: "pregunta_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "respuesta_libre",
                columns: table => new
                {
                    respuesta_id = table.Column<long>(type: "bigint", nullable: false),
                    texto = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_respuesta_libre", x => x.respuesta_id);
                    table.CheckConstraint("ck_respuesta_libre_texto_no_vacio", "length(trim(texto)) > 0");
                    table.ForeignKey(
                        name: "FK_respuesta_libre_respuesta_respuesta_id",
                        column: x => x.respuesta_id,
                        principalTable: "respuesta",
                        principalColumn: "respuesta_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_respuesta_pregunta",
                table: "respuesta",
                column: "pregunta_id");

            migrationBuilder.CreateIndex(
                name: "ux_respuesta_intento",
                table: "respuesta",
                column: "intento_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "respuesta_libre");

            migrationBuilder.DropTable(
                name: "respuesta");
        }
    }
}
