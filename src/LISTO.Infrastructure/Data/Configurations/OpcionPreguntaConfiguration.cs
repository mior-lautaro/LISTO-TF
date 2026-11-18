using LISTO.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LISTO.Infrastructure.Data.Configurations;

public class OpcionPreguntaConfiguration
    : IEntityTypeConfiguration<OpcionPregunta>
{
    public void Configure(EntityTypeBuilder<OpcionPregunta> builder)
    {
        builder.ToTable("opcion_pregunta", table =>
        {
            table.HasCheckConstraint(
                "ck_opcion_pregunta_orden_positivo",
                "orden > 0");
        });

        builder.HasKey(o => o.OpcionPreguntaId);

        builder.Property(o => o.OpcionPreguntaId)
            .HasColumnName("opcion_pregunta_id")
            .UseIdentityByDefaultColumn();

        builder.Property(o => o.PreguntaId)
            .HasColumnName("pregunta_id")
            .IsRequired();

        builder.Property(o => o.Texto)
            .HasColumnName("texto")
            .IsRequired();

        builder.Property(o => o.Orden)
            .HasColumnName("orden")
            .IsRequired();

        builder.Property(o => o.EsCorrecta)
            .HasColumnName("es_correcta")
            .IsRequired();

        builder.HasOne(o => o.Pregunta)
            .WithMany(p => p.Opciones)
            .HasForeignKey(o => o.PreguntaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(o => o.Pregunta);

        builder.HasIndex(o => new
        {
            o.PreguntaId,
            o.Orden
        })
            .IsUnique()
            .HasDatabaseName(
                "ux_opcion_pregunta_pregunta_orden");

        builder.HasIndex(o => o.PreguntaId)
            .IsUnique()
            .HasFilter("es_correcta = TRUE")
            .HasDatabaseName(
                "ux_opcion_pregunta_correcta");
    }
}