
using LISTO.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LISTO.Infrastructure.Persistence.Configurations;

public class EstudianteConfiguration : IEntityTypeConfiguration<Estudiante>
{
    public void Configure(EntityTypeBuilder<Estudiante> builder)
    {
        builder.ToTable("estudiante", table =>
        {
            table.HasCheckConstraint(
                "ck_estudiante_dni_valido",
                "dni ~ '^[0-9]{7,8}$'");
        });

        builder.HasKey(e => e.EstudianteId);

        builder.Property(e => e.EstudianteId)
            .HasColumnName("estudiante_id")
            .UseIdentityByDefaultColumn();

        builder.Property(e => e.UsuarioId)
            .HasColumnName("usuario_id")
            .IsRequired();

        builder.HasIndex(e => e.UsuarioId)
            .IsUnique()
            .HasDatabaseName("ux_estudiante_usuario");

        builder.Property(e => e.DNI)
            .HasColumnName("dni")
            .HasMaxLength(8)
            .IsRequired();

        builder.HasIndex(e => e.DNI)
            .IsUnique()
            .HasDatabaseName("ux_estudiante_dni");

        builder.Property(e => e.FechaAlta)
            .HasColumnName("fecha_alta")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(e => e.EstaActivo)
            .HasColumnName("esta_activo")
            .HasDefaultValue(true)
            .IsRequired();

        builder.HasOne(e => e.Usuario)
            .WithOne()
            .HasForeignKey<Estudiante>(e => e.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
