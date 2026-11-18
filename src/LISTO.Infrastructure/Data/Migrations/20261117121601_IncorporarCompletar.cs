using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LISTO.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class IncorporarCompletar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "modo_correccion_completar",
                table: "pregunta",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "espacio_completar",
                columns: table => new
                {
                    espacio_completar_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    pregunta_id = table.Column<long>(type: "bigint", nullable: false),
                    respuesta_esperada = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    orden = table.Column<int>(type: "integer", nullable: false),
                    ponderacion = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_espacio_completar", x => x.espacio_completar_id);
                    table.ForeignKey(
                        name: "FK_espacio_completar_pregunta_pregunta_id",
                        column: x => x.pregunta_id,
                        principalTable: "pregunta",
                        principalColumn: "pregunta_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "variante_respuesta",
                columns: table => new
                {
                    variante_respuesta_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    espacio_completar_id = table.Column<long>(type: "bigint", nullable: false),
                    texto = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_variante_respuesta", x => x.variante_respuesta_id);
                    table.ForeignKey(
                        name: "FK_variante_respuesta_espacio_completar_espacio_completar_id",
                        column: x => x.espacio_completar_id,
                        principalTable: "espacio_completar",
                        principalColumn: "espacio_completar_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_espacio_completar_pregunta_id_orden",
                table: "espacio_completar",
                columns: new[] { "pregunta_id", "orden" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_variante_respuesta_espacio_completar_id_texto",
                table: "variante_respuesta",
                columns: new[] { "espacio_completar_id", "texto" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "variante_respuesta");

            migrationBuilder.DropTable(
                name: "espacio_completar");

            migrationBuilder.DropColumn(
                name: "modo_correccion_completar",
                table: "pregunta");
        }
    }
}
