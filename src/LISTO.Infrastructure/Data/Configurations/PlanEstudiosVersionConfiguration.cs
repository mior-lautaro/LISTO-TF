
using LISTO.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LISTO.Infrastructure.Persistence.Configurations;

public class PlanEstudiosVersionConfiguration :
    IEntityTypeConfiguration<PlanEstudiosVersion>
{
    public void Configure(EntityTypeBuilder<PlanEstudiosVersion> builder)
    {
        builder.ToTable("plan_estudios_version", table =>
        {
            table.HasCheckConstraint(
                "ck_plan_estudios_version_numero_positivo",
                "numero_version > 0");

            table.HasCheckConstraint(
                "ck_plan_estudios_version_estado_valido",
                "estado IN (1, 2, 3)");

            table.HasCheckConstraint(
                "ck_plan_estudios_version_fechas",
                "fecha_vigencia_hasta IS NULL OR " +
                "(fecha_vigencia_desde IS NOT NULL AND " +
                "fecha_vigencia_hasta >= fecha_vigencia_desde)");

            table.HasCheckConstraint(
                "ck_plan_estudios_version_vigente_con_fecha",
                "estado <> 2 OR fecha_vigencia_desde IS NOT NULL");
        });

        builder.HasKey(v => v.PlanEstudiosVersionId);

        builder.Property(v => v.PlanEstudiosVersionId)
            .HasColumnName("plan_estudios_version_id")
            .UseIdentityByDefaultColumn();

        builder.Property(v => v.PlanEstudiosId)
            .HasColumnName("plan_estudios_id")
            .IsRequired();

        builder.Property(v => v.NumeroVersion)
            .HasColumnName("numero_version")
            .IsRequired();

        builder.HasIndex(v => new
        {
            v.PlanEstudiosId,
            v.NumeroVersion
        })
            .IsUnique()
            .HasDatabaseName("ux_plan_estudios_version_numero");

        builder.HasIndex(v => v.PlanEstudiosId)
            .IsUnique()
            .HasFilter("estado = 2")
            .HasDatabaseName("ux_plan_estudios_version_vigente");

        builder.Property(v => v.FechaVigenciaDesde)
            .HasColumnName("fecha_vigencia_desde")
            .IsRequired(false);

        builder.Property(v => v.FechaVigenciaHasta)
            .HasColumnName("fecha_vigencia_hasta")
            .IsRequired(false);

        builder.Property(v => v.Estado)
            .HasColumnName("estado")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(v => v.Observaciones)
            .HasColumnName("observaciones")
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.HasMany(v => v.Cursos)
            .WithOne()
            .HasForeignKey(c => c.PlanEstudiosVersionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(v => v.Cursos)
            .HasField("_cursos")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
