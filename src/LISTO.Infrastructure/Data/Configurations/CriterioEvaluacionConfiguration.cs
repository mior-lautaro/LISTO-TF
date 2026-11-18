
using LISTO.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LISTO.Infrastructure.Data.Configurations;

public class CriterioEvaluacionConfiguration
    : IEntityTypeConfiguration<CriterioEvaluacion>
{
    public void Configure(
        EntityTypeBuilder<CriterioEvaluacion> builder)
    {
        builder.ToTable("criterio_evaluacion", table =>
        {
            table.HasCheckConstraint(
                "ck_criterio_evaluacion_ponderacion",
                "ponderacion > 0 AND ponderacion <= 100");

            table.HasCheckConstraint(
                "ck_criterio_evaluacion_orden",
                "orden > 0");

            table.HasCheckConstraint(
                "ck_criterio_evaluacion_descripcion_no_vacia",
                "length(trim(descripcion)) > 0");
        });

        builder.HasKey(c => c.CriterioEvaluacionId);

        builder.Property(c => c.CriterioEvaluacionId)
            .HasColumnName("criterio_evaluacion_id")
            .UseIdentityByDefaultColumn();

        builder.Property(c => c.PreguntaId)
            .HasColumnName("pregunta_id")
            .IsRequired();

        builder.Property(c => c.Descripcion)
            .HasColumnName("descripcion")
            .IsRequired();

        builder.Property(c => c.Ponderacion)
            .HasColumnName("ponderacion")
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(c => c.Orden)
            .HasColumnName("orden")
            .IsRequired();

        builder.HasIndex(c => new
        {
            c.PreguntaId,
            c.Orden
        })
            .IsUnique()
            .HasDatabaseName(
                "ux_criterio_evaluacion_pregunta_orden");
    }
}
