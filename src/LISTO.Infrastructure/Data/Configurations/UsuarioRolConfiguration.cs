using LISTO.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LISTO.Infrastructure.Data.Configurations;

public class UsuarioRolConfiguration : IEntityTypeConfiguration<UsuarioRol>
{
    public void Configure(EntityTypeBuilder<UsuarioRol> builder)
    {
        builder.ToTable("usuario_rol", table =>
        {
            table.HasCheckConstraint(
                "ck_usuario_rol_fechas",
                "\"fecha_revocacion\" IS NULL OR " +
                "\"fecha_revocacion\" >= \"fecha_asignacion\"");
        });

        builder.HasKey(ur => ur.UsuarioRolId);

        builder.Property(ur => ur.UsuarioRolId)
            .HasColumnName("usuario_rol_id")
            .ValueGeneratedOnAdd();

        builder.Property(ur => ur.UsuarioId)
            .HasColumnName("usuario_id")
            .IsRequired();

        builder.Property(ur => ur.RolId)
            .HasColumnName("rol_id")
            .IsRequired();

        builder.Property(ur => ur.FechaAsignacion)
            .HasColumnName("fecha_asignacion")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(ur => ur.FechaRevocacion)
            .HasColumnName("fecha_revocacion")
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);

        builder.HasOne(ur => ur.Usuario)
            .WithMany()
            .HasForeignKey(ur => ur.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ur => ur.Rol)
            .WithMany()
            .HasForeignKey(ur => ur.RolId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(ur => new { ur.UsuarioId, ur.RolId })
            .IsUnique()
            .HasFilter("fecha_revocacion IS NULL")
            .HasDatabaseName("ux_usuario_rol_vigente");
    }
}
