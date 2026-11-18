
using LISTO.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LISTO.Infrastructure.Data.Configurations;

public class ConfiguracionRespuestaLibreConfiguration
    : IEntityTypeConfiguration<ConfiguracionRespuestaLibre>
{
    public void Configure(
        EntityTypeBuilder<ConfiguracionRespuestaLibre> builder)
    {
        builder.ToTable("configuracion_respuesta_libre", table =>
        {
            table.HasCheckConstraint(
                "ck_configuracion_respuesta_libre_referencia_no_vacia",
                "length(trim(respuesta_referencia)) > 0");
        });

        builder.HasKey(c => c.PreguntaId);

        builder.Property(c => c.PreguntaId)
            .HasColumnName("pregunta_id")
            .IsRequired();

        builder.Property(c => c.RespuestaReferencia)
            .HasColumnName("respuesta_referencia")
            .IsRequired();

 

        builder.HasMany(c => c.Criterios)
            .WithOne(c => c.ConfiguracionRespuestaLibre)
            .HasForeignKey(c => c.PreguntaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(c => c.Criterios)
            .HasField("_criterios")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
