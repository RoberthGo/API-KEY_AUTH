using APIkey.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Soporte nativo para ProblemDetails (RFC 7807)
builder.Services.AddProblemDetails();

// Configure CORS to allow connections from any origin, method, and header
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Register ContextDB for dependency injection
builder.Services.AddDbContext<ContextDB>();

var app = builder.Build();

// Manejo centralizado de excepciones (500) en formato ProblemDetails
app.UseExceptionHandler();

// Convierte códigos de estado vacíos en ProblemDetails
app.UseStatusCodePages();

app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

// Aplica automáticamente las migraciones pendientes en la BD de Azure al iniciar
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ContextDB>();
    dbContext.Database.Migrate();
}

app.Run();