using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LISTO.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class IncorporarPregunta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "pregunta",
                columns: table => new
                {
                    pregunta_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ejercicio_version_id = table.Column<long>(type: "bigint", nullable: false),
                    enunciado = table.Column<string>(type: "text", nullable: false),
                    tipo = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pregunta", x => x.pregunta_id);
                    table.CheckConstraint("ck_pregunta_tipo_valido", "tipo IN (1, 2, 3, 4)");
                    table.ForeignKey(
                        name: "FK_pregunta_ejercicio_version_ejercicio_version_id",
                        column: x => x.ejercicio_version_id,
                        principalTable: "ejercicio_version",
                        principalColumn: "ejercicio_version_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ux_pregunta_ejercicio_version",
                table: "pregunta",
                column: "ejercicio_version_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "pregunta");
        }
    }
}
