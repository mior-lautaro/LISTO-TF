using LISTO.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LISTO.Infrastructure.Persistence.Configurations;

public class VarianteRespuestaConfiguration
    : IEntityTypeConfiguration<VarianteRespuesta>
{
    public void Configure(EntityTypeBuilder<VarianteRespuesta> builder)
    {
        builder.ToTable("variante_respuesta");

        builder.HasKey(v => v.VarianteRespuestaId);

        builder.Property(v => v.VarianteRespuestaId)
            .HasColumnName("variante_respuesta_id")
            .ValueGeneratedOnAdd();

        builder.Property(v => v.EspacioCompletarId)
            .HasColumnName("espacio_completar_id")
            .IsRequired();

        builder.Property(v => v.Texto)
            .HasColumnName("texto")
            .HasMaxLength(1000)
            .IsRequired();

        builder.HasIndex(v => new { v.EspacioCompletarId, v.Texto })
            .IsUnique();
    }
}