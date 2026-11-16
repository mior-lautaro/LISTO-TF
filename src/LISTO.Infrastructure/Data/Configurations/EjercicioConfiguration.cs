using LISTO.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LISTO.Infrastructure.Data.Configurations;

public class EjercicioConfiguration
    : IEntityTypeConfiguration<Ejercicio>
{
    public void Configure(EntityTypeBuilder<Ejercicio> builder)
    {
        builder.ToTable("ejercicio", table =>
        {
            table.HasCheckConstraint(
                "ck_ejercicio_estado_valido",
                "estado IN (1, 2)");
        });

        builder.HasKey(e => e.EjercicioId);

        builder.Property(e => e.EjercicioId)
            .HasColumnName("ejercicio_id")
            .UseIdentityByDefaultColumn();

        builder.Property(e => e.CursoId)
            .HasColumnName("curso_id")
            .IsRequired();

        builder.Property(e => e.FechaCreacion)
            .HasColumnName("fecha_creacion")
            .IsRequired();

        builder.Property(e => e.Estado)
            .HasColumnName("estado")
            .HasConversion<int>()
            .IsRequired();

        builder.HasOne(e => e.Curso)
            .WithMany()
            .HasForeignKey(e => e.CursoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.CursoId)
            .HasDatabaseName("ix_ejercicio_curso_id");
    }
}