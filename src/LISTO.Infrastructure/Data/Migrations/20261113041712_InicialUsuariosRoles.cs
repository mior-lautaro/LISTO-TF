using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LISTO.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InicialUsuariosRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "rol",
                columns: table => new
                {
                    rol_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    esta_activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    nombre_normalizado = table.Column<string>(type: "text", nullable: true, computedColumnSql: "lower(btrim(nombre))", stored: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rol", x => x.rol_id);
                    table.CheckConstraint("ck_rol_nombre_no_vacio", "length(btrim(\"nombre\")) > 0");
                });

            migrationBuilder.CreateTable(
                name: "usuario",
                columns: table => new
                {
                    usuario_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    email = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    apellido = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    hash_contrasena = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    esta_activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    fecha_alta = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuario", x => x.usuario_id);
                    table.CheckConstraint("ck_usuario_apellido_no_vacio", "length(btrim(\"apellido\")) > 0");
                    table.CheckConstraint("ck_usuario_email_valido", "length(btrim(\"email\")) > 0 AND \"email\" = lower(btrim(\"email\"))");
                    table.CheckConstraint("ck_usuario_nombre_no_vacio", "length(btrim(\"nombre\")) > 0");
                });

            migrationBuilder.CreateTable(
                name: "usuario_rol",
                columns: table => new
                {
                    usuario_rol_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usuario_id = table.Column<long>(type: "bigint", nullable: false),
                    rol_id = table.Column<long>(type: "bigint", nullable: false),
                    fecha_asignacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    fecha_revocacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuario_rol", x => x.usuario_rol_id);
                    table.CheckConstraint("ck_usuario_rol_fechas", "\"fecha_revocacion\" IS NULL OR \"fecha_revocacion\" >= \"fecha_asignacion\"");
                    table.ForeignKey(
                        name: "FK_usuario_rol_rol_rol_id",
                        column: x => x.rol_id,
                        principalTable: "rol",
                        principalColumn: "rol_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_usuario_rol_usuario_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuario",
                        principalColumn: "usuario_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ux_rol_nombre_normalizado",
                table: "rol",
                column: "nombre_normalizado",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_usuario_email",
                table: "usuario",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuario_rol_rol_id",
                table: "usuario_rol",
                column: "rol_id");

            migrationBuilder.CreateIndex(
                name: "ux_usuario_rol_vigente",
                table: "usuario_rol",
                columns: new[] { "usuario_id", "rol_id" },
                unique: true,
                filter: "fecha_revocacion IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "usuario_rol");

            migrationBuilder.DropTable(
                name: "rol");

            migrationBuilder.DropTable(
                name: "usuario");
        }
    }
}
