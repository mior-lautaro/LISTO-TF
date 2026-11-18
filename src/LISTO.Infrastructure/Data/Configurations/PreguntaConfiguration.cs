
using LISTO.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LISTO.Infrastructure.Data.Configurations;

public class PreguntaConfiguration
    : IEntityTypeConfiguration<Pregunta>
{
    public void Configure(EntityTypeBuilder<Pregunta> builder)
    {
        builder.ToTable("pregunta", table =>
        {
            table.HasCheckConstraint(
                "ck_pregunta_tipo_valido",
                "tipo IN (1, 2, 3, 4)");
        });

        builder.HasKey(p => p.PreguntaId);

        builder.Property(p => p.PreguntaId)
            .HasColumnName("pregunta_id")
            .UseIdentityByDefaultColumn();

        builder.Property(p => p.EjercicioVersionId)
            .HasColumnName("ejercicio_version_id")
            .IsRequired();

        builder.Property(p => p.Enunciado)
            .HasColumnName("enunciado")
            .IsRequired();

        builder.Property(p => p.Tipo)
            .HasColumnName("tipo")
            .HasConversion<int>()
            .IsRequired();

        builder.HasOne(p => p.EjercicioVersion)
            .WithOne(v => v.Pregunta)
            .HasForeignKey<Pregunta>(
                p => p.EjercicioVersionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.EjercicioVersionId)
            .IsUnique()
            .HasDatabaseName(
                "ux_pregunta_ejercicio_version");

        builder.Property(p => p.ModoCorreccionCompletar)
            .HasColumnName("modo_correccion_completar")
            .HasConversion<int>()
            .IsRequired(false);

        // Relación uno a uno con ConfiguracionRespuestaLibre.
        builder.HasOne(p => p.ConfiguracionRespuestaLibre)
            .WithOne(c => c.Pregunta)
            .HasForeignKey<ConfiguracionRespuestaLibre>(
                c => c.PreguntaId)
            .OnDelete(DeleteBehavior.Restrict);

        // Acceso mediante el campo privado del dominio.
        builder.Navigation(p => p.ConfiguracionRespuestaLibre)
            .HasField("_configuracionRespuestaLibre")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(p => p.EspaciosCompletar)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
