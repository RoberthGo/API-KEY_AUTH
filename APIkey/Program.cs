using APIkey.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Soporte nativo para ProblemDetails (RFC 7807)
builder.Services.AddProblemDetails();

// Configuración de Swagger / OpenAPI con soporte para X-API-KEY
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "Introduce tu API Key en este formato:",
        Type = SecuritySchemeType.ApiKey,
        Name = "X-API-KEY",
        In = ParameterLocation.Header
    });

    c.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("ApiKey", doc)] = new List<string>()
    });
});

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

// Configure the HTTP request pipeline.
// Swagger habilitado para todos los entornos (incluido Azure)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Escolar v1");
    c.RoutePrefix = string.Empty;
});

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
