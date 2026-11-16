
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LISTO.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class IncorporarSecuenciaCodigoPlanEstudios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "plan_estudios_codigo_seq",
                startValue: 1L,
                incrementBy: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(
                name: "plan_estudios_codigo_seq");
        }
    }
}

