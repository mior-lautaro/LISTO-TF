using LISTO.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LISTO.Infrastructure.Persistence.Configurations;

public class PlanEstudiosConfiguration : IEntityTypeConfiguration<PlanEstudios>
{
    public void Configure(EntityTypeBuilder<PlanEstudios> builder)
    {
        builder.ToTable("plan_estudios", table =>
        {
            table.HasCheckConstraint(
                "ck_plan_estudios_codigo_no_vacio",
                "btrim(codigo) <> ''");

            table.HasCheckConstraint(
                "ck_plan_estudios_nombre_no_vacio",
                "btrim(nombre) <> ''");
        });

        builder.HasKey(p => p.PlanEstudiosId);

        builder.Property(p => p.PlanEstudiosId)
            .HasColumnName("plan_estudios_id")
            .UseIdentityByDefaultColumn();

        builder.Property(p => p.Codigo)
            .HasColumnName("codigo")
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(p => p.Codigo)
            .IsUnique()
            .HasDatabaseName("ux_plan_estudios_codigo");

        builder.Property(p => p.Nombre)
            .HasColumnName("nombre")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(p => p.Descripcion)
            .HasColumnName("descripcion")
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(p => p.EstaActivo)
            .HasColumnName("esta_activo")
            .HasDefaultValue(true)
            .IsRequired();

        builder.HasMany(p => p.Versiones)
            .WithOne()
            .HasForeignKey(v => v.PlanEstudiosId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(p => p.Versiones)
            .HasField("_versiones")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
