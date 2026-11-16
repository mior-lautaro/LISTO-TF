using System;
using System.Collections.Generic;
using System.Text;
using LISTO.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LISTO.Infrastructure.Persistence.Configurations;

public class CursoConfiguration : IEntityTypeConfiguration<Curso>
{
    public void Configure(EntityTypeBuilder<Curso> builder)
    {
        builder.ToTable("curso", table =>
        {
            table.HasCheckConstraint(
                "ck_curso_codigo_no_vacio",
                "btrim(codigo) <> ''");

            table.HasCheckConstraint(
                "ck_curso_nombre_no_vacio",
                "btrim(nombre) <> ''");
        });

        builder.HasKey(c => c.CursoId);

        builder.Property(c => c.CursoId)
            .HasColumnName("curso_id")
            .UseIdentityByDefaultColumn();

        builder.Property(c => c.Codigo)
            .HasColumnName("codigo")
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(c => c.Codigo)
            .IsUnique()
            .HasDatabaseName("ux_curso_codigo");

        builder.Property(c => c.Nombre)
            .HasColumnName("nombre")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(c => c.Descripcion)
            .HasColumnName("descripcion")
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(c => c.EstaActivo)
            .HasColumnName("esta_activo")
            .HasDefaultValue(true)
            .IsRequired();
    }
}
