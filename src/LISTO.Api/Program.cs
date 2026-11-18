using LISTO.Application.Interfaces;
using LISTO.Infrastructure.Repositories;
using LISTO.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using LISTO.Application.Services;
using LISTO.Infrastructure.Security;
using LISTO.Infrastructure.Generators;

var builder = WebApplication.CreateBuilder(args);



// Configurar la conexión a PostgreSQL.
var connectionString = builder.Configuration
    .GetConnectionString("ListoDb");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "No se encontró la cadena de conexión ListoDb.");
}

const string passwordPlaceholder = "${LISTO_DB_PASSWORD}";

if (connectionString.Contains(passwordPlaceholder))
{
    var password = Environment.GetEnvironmentVariable(
        "LISTO_DB_PASSWORD");

    if (string.IsNullOrWhiteSpace(password))
    {
        throw new InvalidOperationException(
            "No se encontró la variable de entorno LISTO_DB_PASSWORD.");
    }

    connectionString = connectionString.Replace(
        passwordPlaceholder,
        password);
}

builder.Services.AddDbContext<ListoDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IRolRepository, RolRepository>();
builder.Services.AddScoped<IUsuarioRolRepository, UsuarioRolRepository>();
builder.Services.AddScoped<ICursoRepository, CursoRepository>();
builder.Services.AddScoped<ICursoService, CursoService>();
builder.Services.AddScoped<IPlanEstudiosService, PlanEstudiosService>();
builder.Services.AddScoped<IPlanEstudiosVersionService, PlanEstudiosVersionService>();
builder.Services.AddScoped<IPlanEstudiosRepository, PlanEstudiosRepository>();
builder.Services.AddScoped<IEstudianteRepository, EstudianteRepository>();
builder.Services.AddScoped<IInscripcionRepository, InscripcionRepository>();
builder.Services.AddScoped<IEstudianteService, EstudianteService>();
builder.Services.AddScoped<IInscripcionService, InscripcionService>();
builder.Services.AddScoped<IPasswordHasher, IdentityPasswordHasher>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IRolService, RolService>();
builder.Services.AddScoped<IUsuarioRolService, UsuarioRolService>();
builder.Services.AddScoped<
    ICodigoInstitucionalGenerator,
    PostgresCodigoInstitucionalGenerator>();
builder.Services.AddScoped<IEjercicioService, EjercicioService>();
builder.Services.AddScoped<IIntentoService, IntentoService>();
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddScoped<IEjercicioRepository, EjercicioRepository>();
builder.Services.AddScoped<IIntentoRepository, IntentoRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/api/diagnostico/base-datos", async (
    ListoDbContext db,
    ILogger<Program> logger) =>
{
    try
    {
        await db.Database.OpenConnectionAsync();
        await db.Database.CloseConnectionAsync();

        return Results.Ok(new
        {
            baseDatos = "PostgreSQL",
            estado = "Conexión exitosa"
        });
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error al conectar con PostgreSQL");

        return Results.Problem(
            title: "Error de conexión con PostgreSQL",
            detail: "Revisá la consola de la API para conocer la causa.");
    }
});
app.Run();
public partial class Program { }
