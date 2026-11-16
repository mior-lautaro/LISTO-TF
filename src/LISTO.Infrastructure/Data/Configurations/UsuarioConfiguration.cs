using LISTO.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LISTO.Infrastructure.Data.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("usuario", table =>
        {
            table.HasCheckConstraint(
                "ck_usuario_email_valido",
                "length(btrim(\"email\")) > 0 AND " +
                "\"email\" = lower(btrim(\"email\"))");

            table.HasCheckConstraint(
                "ck_usuario_nombre_no_vacio",
                "length(btrim(\"nombre\")) > 0");

            table.HasCheckConstraint(
                "ck_usuario_apellido_no_vacio",
                "length(btrim(\"apellido\")) > 0");
        });

        builder.HasKey(u => u.UsuarioId);

        builder.Property(u => u.UsuarioId)
            .HasColumnName("usuario_id")
            .ValueGeneratedOnAdd();

        builder.Property(u => u.Email)
            .HasColumnName("email")
            .HasMaxLength(254)
            .IsRequired();

        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("ux_usuario_email");

        builder.Property(u => u.Nombre)
            .HasColumnName("nombre")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.Apellido)
            .HasColumnName("apellido")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.HashContrasena)
            .HasColumnName("hash_contrasena")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(u => u.EstaActivo)
            .HasColumnName("esta_activo")
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(u => u.FechaAlta)
            .HasColumnName("fecha_alta")
            .HasColumnType("timestamp with time zone")
            .IsRequired();
    }
}