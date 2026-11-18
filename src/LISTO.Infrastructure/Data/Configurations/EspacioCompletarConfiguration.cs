using LISTO.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LISTO.Infrastructure.Persistence.Configurations;

public class EspacioCompletarConfiguration
    : IEntityTypeConfiguration<EspacioCompletar>
{
    public void Configure(EntityTypeBuilder<EspacioCompletar> builder)
    {
        builder.ToTable("espacio_completar");

        builder.HasKey(e => e.EspacioCompletarId);

        builder.Property(e => e.EspacioCompletarId)
            .HasColumnName("espacio_completar_id")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.PreguntaId)
            .HasColumnName("pregunta_id")
            .IsRequired();

        builder.Property(e => e.RespuestaEsperada)
            .HasColumnName("respuesta_esperada")
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(e => e.Orden)
            .HasColumnName("orden")
            .IsRequired();

        builder.Property(e => e.Ponderacion)
            .HasColumnName("ponderacion")
            .HasPrecision(5, 2);

        builder.HasIndex(e => new { e.PreguntaId, e.Orden })
            .IsUnique();

        builder.HasOne(e => e.Pregunta)
            .WithMany(p => p.EspaciosCompletar)
            .HasForeignKey(e => e.PreguntaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.Variantes)
            .WithOne(v => v.EspacioCompletar)
            .HasForeignKey(v => v.EspacioCompletarId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(e => e.Variantes)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}