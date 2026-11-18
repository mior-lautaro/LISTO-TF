using LISTO.Domain.Entities;
using LISTO.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using LISTO.Infrastructure.Persistence.Configurations;

namespace LISTO.Infrastructure.Data;

public class ListoDbContext : DbContext
{
    public ListoDbContext(DbContextOptions<ListoDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();

    public DbSet<Rol> Roles => Set<Rol>();

    public DbSet<Curso> Cursos => Set<Curso>();

    public DbSet<UsuarioRol> UsuariosRoles => Set<UsuarioRol>();

    public DbSet<PlanEstudios> PlanesEstudios => Set<PlanEstudios>();

    public DbSet<PlanEstudiosVersion> PlanesEstudiosVersiones =>
        Set<PlanEstudiosVersion>();

    public DbSet<PlanCurso> PlanesCursos => Set<PlanCurso>();


    public DbSet<Estudiante> Estudiantes => Set<Estudiante>();

    public DbSet<Inscripcion> Inscripciones => Set<Inscripcion>();

    public DbSet<Ejercicio> Ejercicios => Set<Ejercicio>();

    public DbSet<EspacioCompletar> EspaciosCompletar => Set<EspacioCompletar>();
    public DbSet<VarianteRespuesta> VariantesRespuesta => Set<VarianteRespuesta>();

    public DbSet<Intento> Intentos => Set<Intento>();

    public DbSet<EjercicioVersion> EjerciciosVersiones =>
    Set<EjercicioVersion>();

    public DbSet<Respuesta> Respuestas => Set<Respuesta>();

    public DbSet<RespuestaLibre> RespuestasLibres => Set<RespuestaLibre>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UsuarioConfiguration());
        modelBuilder.ApplyConfiguration(new RolConfiguration());
        modelBuilder.ApplyConfiguration(new UsuarioRolConfiguration());
        modelBuilder.ApplyConfiguration(new CursoConfiguration());

        modelBuilder.ApplyConfiguration(new PlanEstudiosConfiguration());
        modelBuilder.ApplyConfiguration(new PlanEstudiosVersionConfiguration());
        modelBuilder.ApplyConfiguration(new PlanCursoConfiguration());

        modelBuilder.ApplyConfiguration(new EstudianteConfiguration());
        modelBuilder.ApplyConfiguration(new InscripcionConfiguration());

        modelBuilder.ApplyConfiguration(new EjercicioConfiguration());
        modelBuilder.ApplyConfiguration(new EjercicioVersionConfiguration());
        modelBuilder.ApplyConfiguration(new PreguntaConfiguration());
        modelBuilder.ApplyConfiguration(new ConfiguracionRespuestaLibreConfiguration());

        modelBuilder.ApplyConfiguration(new CriterioEvaluacionConfiguration());
        modelBuilder.ApplyConfiguration(new OpcionPreguntaConfiguration());
        modelBuilder.ApplyConfiguration(new ConfiguracionVerdaderoFalsoConfiguration());
        modelBuilder.ApplyConfiguration(new EspacioCompletarConfiguration());
        modelBuilder.ApplyConfiguration(new VarianteRespuestaConfiguration());
        modelBuilder.ApplyConfiguration(new IntentoConfiguration());
        modelBuilder.ApplyConfiguration(new RespuestaConfiguration());
        modelBuilder.ApplyConfiguration(new RespuestaLibreConfiguration());

    }
}
