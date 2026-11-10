
using LISTO.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configurar la conexión a PostgreSQL.
var connectionString = builder.Configuration
    .GetConnectionString("ListoDb");

var password = Environment.GetEnvironmentVariable("LISTO_DB_PASSWORD");

if (string.IsNullOrWhiteSpace(password))
{
    throw new InvalidOperationException(
        "No se encontró la variable de entorno LISTO_DB_PASSWORD.");
}

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "No se encontró la cadena de conexión ListoDb.");
}

connectionString = connectionString.Replace(
    "${LISTO_DB_PASSWORD}",
    password);

builder.Services.AddDbContext<ListoDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllers();
builder.Services.AddOpenApi();

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
