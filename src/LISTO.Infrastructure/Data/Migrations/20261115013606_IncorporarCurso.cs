using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LISTO.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class IncorporarCurso : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "curso_codigo_seq",
                startValue: 1L,
                incrementBy: 1);
            migrationBuilder.CreateTable(
                name: "curso",
                columns: table => new
                {
                    curso_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    esta_activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_curso", x => x.curso_id);
                    table.CheckConstraint("ck_curso_codigo_no_vacio", "btrim(codigo) <> ''");
                    table.CheckConstraint("ck_curso_nombre_no_vacio", "btrim(nombre) <> ''");
                });

            migrationBuilder.CreateIndex(
                name: "ux_curso_codigo",
                table: "curso",
                column: "codigo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "curso");
            migrationBuilder.DropSequence(
                name: "curso_codigo_seq");
        }
    }
}
