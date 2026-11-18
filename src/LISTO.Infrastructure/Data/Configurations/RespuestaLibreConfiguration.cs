
using LISTO.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LISTO.Infrastructure.Data.Configurations;

public class RespuestaLibreConfiguration
    : IEntityTypeConfiguration<RespuestaLibre>
{
    public void Configure(EntityTypeBuilder<RespuestaLibre> builder)
    {
        builder.ToTable("respuesta_libre", table =>
        {
            table.HasCheckConstraint(
                "ck_respuesta_libre_texto_no_vacio",
                "length(trim(texto)) > 0");
        });

        builder.HasKey(r => r.RespuestaId);

        builder.Property(r => r.RespuestaId)
            .HasColumnName("respuesta_id");

        builder.Property(r => r.Texto)
            .HasColumnName("texto")
            .IsRequired();

        builder.HasOne(r => r.Respuesta)
            .WithOne(r => r.RespuestaLibre)
            .HasForeignKey<RespuestaLibre>(r => r.RespuestaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
