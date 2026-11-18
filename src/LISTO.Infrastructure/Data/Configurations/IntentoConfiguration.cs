using LISTO.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LISTO.Infrastructure.Data.Configurations;

public class IntentoConfiguration
    : IEntityTypeConfiguration<Intento>
{
    public void Configure(EntityTypeBuilder<Intento> builder)
    {
        builder.ToTable("intento", table =>
        {
            table.HasCheckConstraint(
                "ck_intento_estado_valido",
                "estado IN (1, 2, 3)");

            table.HasCheckConstraint(
                "ck_intento_fechas_estado",
                "(estado = 1 AND fecha_envio IS NULL " +
                "AND fecha_evaluacion IS NULL) OR " +
                "(estado = 2 AND fecha_envio IS NOT NULL " +
                "AND fecha_evaluacion IS NULL) OR " +
                "(estado = 3 AND fecha_envio IS NOT NULL " +
                "AND fecha_evaluacion IS NOT NULL)");
        });

        builder.HasKey(i => i.IntentoId);

        builder.Property(i => i.IntentoId)
            .HasColumnName("intento_id")
            .UseIdentityByDefaultColumn();

        builder.Property(i => i.EstudianteId)
            .HasColumnName("estudiante_id")
            .IsRequired();

        builder.Property(i => i.EjercicioId)
            .HasColumnName("ejercicio_id")
            .IsRequired();

        builder.Property(i => i.EjercicioVersionId)
            .HasColumnName("ejercicio_version_id")
            .IsRequired();

        builder.Property(i => i.Estado)
            .HasColumnName("estado")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(i => i.FechaInicio)
            .HasColumnName("fecha_inicio")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(i => i.FechaEnvio)
            .HasColumnName("fecha_envio")
            .HasColumnType("timestamp with time zone");

        builder.Property(i => i.FechaEvaluacion)
            .HasColumnName("fecha_evaluacion")
            .HasColumnType("timestamp with time zone");

        builder.HasOne(i => i.Estudiante)
            .WithMany()
            .HasForeignKey(i => i.EstudianteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Ejercicio)
            .WithMany()
            .HasForeignKey(i => i.EjercicioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.EjercicioVersion)
            .WithMany()
            .HasForeignKey(i => i.EjercicioVersionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(i => new
        {
            i.EstudianteId,
            i.EjercicioId
        })
            .HasDatabaseName("ix_intento_estudiante_ejercicio");

        builder.HasIndex(i => new
        {
            i.EstudianteId,
            i.EjercicioId
        })
            .IsUnique()
            .HasFilter("estado = 1")
            .HasDatabaseName(
                "ux_intento_estudiante_ejercicio_en_curso");
    }
}