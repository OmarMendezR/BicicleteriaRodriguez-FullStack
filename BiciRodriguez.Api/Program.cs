using BiciRodriguez.Api.Models; // Importa tus tablas (Bicicleta, Cliente, etc.)
using Microsoft.EntityFrameworkCore;
using BiciRodriguez.Api.Services; // Importa tus servicios personalizados (BicicletasService)

var builder = WebApplication.CreateBuilder(args);

#region 1. CONFIGURACIÓN DE SERVICIOS (Dependency Injection)
// ----------------------------------------------------------------------------------

// --- Base de Datos ---
// Registra el contexto 'BiciContext' usando la cadena de conexión de appsettings.json
builder.Services.AddDbContext<BiciContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- Controladores y JSON ---
builder.Services.AddControllers()
    .AddJsonOptions(options => {
        // Crucial: Evita errores de "Ciclo Infinito" al consultar tablas relacionadas
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// --- Swagger / OpenAPI (Documentación Interactiva) ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Usamos la ruta completa 'Microsoft.OpenApi.Models.OpenApiInfo' 
    // para evitar conflictos con tu carpeta local 'Models'
    c.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "Bicicletería Rodríguez API",
        Version = "v1",
        Description = "Sistema de Gestión de Taller y Ventas - OmarMendezR"
    });
});

// --- Seguridad CORS (ISO 27001) ---
// Permite que tu futuro Front-end (React/Angular) se conecte a esta API sin bloqueos
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

// ----------------------------------------------------------------------------------
#endregion
#region 1.5 CONFIGURACIÓN DE SERVICIOS PERSONALIZADOS (Inyección de Dependencias)

builder.Services.AddScoped<IBicicletasService, BicicletasService>();
builder.Services.AddScoped<IClientesService, ClientesService>();
builder.Services.AddScoped<IFichasService, FichasService>();
builder.Services.AddScoped<IProductosService, ProductosService>();
builder.Services.AddScoped<IUsuariosService, UsuariosService>();

#endregion
var app = builder.Build();

#region 2. PIPELINE DE MIDDLEWARE (Configuración de Red)
// ----------------------------------------------------------------------------------

// Habilitar Swagger solo en entorno de Desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); // Fuerza el uso de conexiones seguras (HTTPS)

app.UseCors("AllowAll");    // Aplica la política de acceso configurada arriba

app.UseAuthorization();    // Prepara el terreno para el sistema de Login/Roles

// ----------------------------------------------------------------------------------
#endregion

#region 3. MAPEADO DE RUTAS (Endpoints)
// ----------------------------------------------------------------------------------

// Registra automáticamente todos los Controllers que crearemos
app.MapControllers();

// Endpoint de Salud (Health Check): Útil para monitorear que la API esté "viva"
app.MapGet("/health", () => Results.Ok(new
{
    status = "Healthy",
    timestamp = DateTime.UtcNow,
    db_status = "Connected"
}));

// ----------------------------------------------------------------------------------
#endregion

app.Run();