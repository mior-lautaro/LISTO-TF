
using LISTO.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LISTO.Infrastructure.Data.Configurations;

public class RespuestaConfiguration
    : IEntityTypeConfiguration<Respuesta>
{
    public void Configure(EntityTypeBuilder<Respuesta> builder)
    {
        builder.ToTable("respuesta");

        builder.HasKey(r => r.RespuestaId);

        builder.Property(r => r.RespuestaId)
            .HasColumnName("respuesta_id")
            .UseIdentityByDefaultColumn();

        builder.Property(r => r.IntentoId)
            .HasColumnName("intento_id")
            .IsRequired();

        builder.Property(r => r.PreguntaId)
            .HasColumnName("pregunta_id")
            .IsRequired();

        builder.Property(r => r.FechaRegistro)
            .HasColumnName("fecha_registro")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.HasOne(r => r.Intento)
            .WithOne(i => i.Respuesta)
            .HasForeignKey<Respuesta>(r => r.IntentoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Pregunta)
            .WithMany()
            .HasForeignKey(r => r.PreguntaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(r => r.IntentoId)
            .IsUnique()
            .HasDatabaseName("ux_respuesta_intento");

        builder.HasIndex(r => r.PreguntaId)
            .HasDatabaseName("ix_respuesta_pregunta");
    }
}
