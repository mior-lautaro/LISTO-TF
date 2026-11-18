
using LISTO.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LISTO.Infrastructure.Data.Configurations;

public class EjercicioVersionConfiguration
    : IEntityTypeConfiguration<EjercicioVersion>
{
    public void Configure(EntityTypeBuilder<EjercicioVersion> builder)
    {
        builder.ToTable("ejercicio_version", table =>
        {
            table.HasCheckConstraint(
                "ck_ejercicio_version_numero_positivo",
                "numero_version > 0");

            table.HasCheckConstraint(
                "ck_ejercicio_version_estado_valido",
                "estado IN (1, 2, 3, 4)");
        });

        builder.HasKey(v => v.EjercicioVersionId);

        builder.Property(v => v.EjercicioVersionId)
            .HasColumnName("ejercicio_version_id")
            .UseIdentityByDefaultColumn();

        builder.Property(v => v.EjercicioId)
            .HasColumnName("ejercicio_id")
            .IsRequired();

        builder.Property(v => v.NumeroVersion)
            .HasColumnName("numero_version")
            .IsRequired();

        builder.Property(v => v.FechaCreacion)
            .HasColumnName("fecha_creacion")
            .IsRequired();

        builder.Property(v => v.FechaPublicacion)
            .HasColumnName("fecha_publicacion");

        builder.Property(v => v.Estado)
            .HasColumnName("estado")
            .HasConversion<int>()
            .IsRequired();

        builder.HasOne(v => v.Ejercicio)
            .WithMany(e => e.Versiones)
            .HasForeignKey(v => v.EjercicioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(v => new
        {
            v.EjercicioId,
            v.NumeroVersion
        })
            .IsUnique()
            .HasDatabaseName(
                "ux_ejercicio_version_ejercicio_numero");

        builder.HasIndex(v => v.EjercicioId)
            .IsUnique()
            .HasFilter("estado = 3")
            .HasDatabaseName(
                "ux_ejercicio_version_publicada");
    }
}
