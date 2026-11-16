
using LISTO.Domain.Entities;
using LISTO.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LISTO.Infrastructure.Persistence.Configurations;

public class InscripcionConfiguration : IEntityTypeConfiguration<Inscripcion>
{
    public void Configure(EntityTypeBuilder<Inscripcion> builder)
    {
        builder.ToTable("inscripcion", table =>
        {
            table.HasCheckConstraint(
                "ck_inscripcion_estado_valido",
                "estado IN (1, 2, 3, 4, 5)");

            table.HasCheckConstraint(
                "ck_inscripcion_fecha_resolucion",
                "(estado = 1 AND fecha_resolucion IS NULL) OR " +
                "(estado IN (2, 3, 4, 5) AND fecha_resolucion IS NOT NULL)");
        });

        builder.HasKey(i => i.InscripcionId);

        builder.Property(i => i.InscripcionId)
            .HasColumnName("inscripcion_id")
            .UseIdentityByDefaultColumn();

        builder.Property(i => i.EstudianteId)
            .HasColumnName("estudiante_id")
            .IsRequired();

        builder.Property(i => i.PlanCursoId)
            .HasColumnName("plan_curso_id")
            .IsRequired();

        builder.Property(i => i.FechaInscripcion)
            .HasColumnName("fecha_inscripcion")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(i => i.Estado)
            .HasColumnName("estado")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(i => i.FechaResolucion)
            .HasColumnName("fecha_resolucion")
            .HasColumnType("timestamp with time zone");

        builder.HasIndex(i => new
        {
            i.EstudianteId,
            i.PlanCursoId
        })
            .IsUnique()
            .HasFilter("estado = 1")
            .HasDatabaseName("ux_inscripcion_estudiante_plan_curso_activa");

        builder.HasOne(i => i.Estudiante)
            .WithMany()
            .HasForeignKey(i => i.EstudianteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.PlanCurso)
            .WithMany()
            .HasForeignKey(i => i.PlanCursoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
