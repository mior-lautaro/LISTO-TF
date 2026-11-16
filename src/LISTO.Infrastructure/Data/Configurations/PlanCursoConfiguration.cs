using LISTO.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LISTO.Infrastructure.Persistence.Configurations;

public class PlanCursoConfiguration : IEntityTypeConfiguration<PlanCurso>
{
    public void Configure(EntityTypeBuilder<PlanCurso> builder)
    {
        builder.ToTable("plan_curso", table =>
        {
            table.HasCheckConstraint(
                "ck_plan_curso_anio_nivel_positivo",
                "anio_nivel > 0");

            table.HasCheckConstraint(
                "ck_plan_curso_periodo_valido",
                "periodo_recomendado IN (1, 2, 3)");

            table.HasCheckConstraint(
                "ck_plan_curso_carga_horaria_positiva",
                "carga_horaria > 0");
        });

        builder.HasKey(pc => pc.PlanCursoId);

        builder.Property(pc => pc.PlanCursoId)
            .HasColumnName("plan_curso_id")
            .UseIdentityByDefaultColumn();

        builder.Property(pc => pc.PlanEstudiosVersionId)
            .HasColumnName("plan_estudios_version_id")
            .IsRequired();

        builder.Property(pc => pc.CursoId)
            .HasColumnName("curso_id")
            .IsRequired();

        builder.HasIndex(pc => new
            {
                pc.PlanEstudiosVersionId,
                pc.CursoId
            })
            .IsUnique()
            .HasDatabaseName("ux_plan_curso_version_curso");

        builder.Property(pc => pc.AnioNivel)
            .HasColumnName("anio_nivel")
            .IsRequired();

        builder.Property(pc => pc.PeriodoRecomendado)
            .HasColumnName("periodo_recomendado")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(pc => pc.CargaHoraria)
            .HasColumnName("carga_horaria")
            .HasPrecision(8, 2)
            .IsRequired();

        builder.HasOne(pc => pc.Curso)
            .WithMany()
            .HasForeignKey(pc => pc.CursoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
