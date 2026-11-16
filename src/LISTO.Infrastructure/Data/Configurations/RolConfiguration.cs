using LISTO.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LISTO.Infrastructure.Data.Configurations;

public class RolConfiguration : IEntityTypeConfiguration<Rol>
{
    public void Configure(EntityTypeBuilder<Rol> builder)
    {
        builder.ToTable("rol", table =>
        {
            table.HasCheckConstraint(
                "ck_rol_nombre_no_vacio",
                "length(btrim(\"nombre\")) > 0");
        });

        builder.HasKey(r => r.RolId);

        builder.Property(r => r.RolId)
            .HasColumnName("rol_id")
            .ValueGeneratedOnAdd();

        builder.Property(r => r.Nombre)
            .HasColumnName("nombre")
            .HasMaxLength(100)
            .IsRequired();

        // Propiedad sombra: no forma parte de la entidad Rol.
        // PostgreSQL calcula su valor automáticamente.
        builder.Property<string>("NombreNormalizado")
            .HasColumnName("nombre_normalizado")
            .HasComputedColumnSql(
                "lower(btrim(nombre))",
                stored: true);

        builder.HasIndex("NombreNormalizado")
            .IsUnique()
            .HasDatabaseName("ux_rol_nombre_normalizado");

        builder.Property(r => r.Descripcion)
            .HasColumnName("descripcion")
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(r => r.EstaActivo)
            .HasColumnName("esta_activo")
            .IsRequired()
            .HasDefaultValue(true);
    }
}