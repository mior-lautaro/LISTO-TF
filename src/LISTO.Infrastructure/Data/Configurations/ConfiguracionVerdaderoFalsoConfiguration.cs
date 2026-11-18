using LISTO.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LISTO.Infrastructure.Data.Configurations;

public class ConfiguracionVerdaderoFalsoConfiguration
    : IEntityTypeConfiguration<ConfiguracionVerdaderoFalso>
{
    public void Configure(
        EntityTypeBuilder<ConfiguracionVerdaderoFalso> builder)
    {
        builder.ToTable("configuracion_verdadero_falso");

        builder.HasKey(c => c.PreguntaId);

        builder.Property(c => c.PreguntaId)
            .HasColumnName("pregunta_id")
            .IsRequired();

        builder.Property(c => c.RespuestaCorrecta)
            .HasColumnName("respuesta_correcta")
            .IsRequired();

        builder.HasOne(c => c.Pregunta)
            .WithOne(p => p.ConfiguracionVerdaderoFalso)
            .HasForeignKey<ConfiguracionVerdaderoFalso>(
                c => c.PreguntaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}