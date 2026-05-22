using BiciRodriguez.Api.Interfaces;
using BiciRodriguez.Api.Models;
using BiciRodriguez.Api.Services;
using BiciRodriguez.Api.Workers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

#region 1. CONFIGURACIÓN DE SERVICIOS

// --- Base de Datos ---
builder.Services.AddDbContext<BiciContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- Autenticación y Seguridad JWT ---
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.MapInboundClaims = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
        ClockSkew = TimeSpan.FromMinutes(5),
        RoleClaimType = "role",
        NameClaimType = "email"
    };
});

// --- Controladores y Serialización ---
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// --- Documentación Swagger ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Bicicletería Rodríguez API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Pega tu token JWT aquí."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// --- Cross-Origin Resource Sharing (CORS) ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

    options.AddPolicy("AllowAngular",
        policy => policy.WithOrigins("http://localhost:4200")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
});

// --- Inyección de Dependencias (Business Logic) ---
builder.Services.AddScoped<IBicicletasService, BicicletasService>();
builder.Services.AddScoped<IClientesService, ClientesService>();
builder.Services.AddScoped<IFichasService, FichasService>();
builder.Services.AddScoped<IProductosService, ProductosService>();
builder.Services.AddScoped<IUsuariosService, UsuariosService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IComprasService, ComprasService>();
builder.Services.AddScoped<IBalancesService, BalancesService>();
builder.Services.AddHostedService<CierreCajaWorker>();
builder.Services.AddScoped<ICategoriasService, CategoriasService>();
builder.Services.AddScoped<IProveedoresService, ProveedoresService>();

#endregion

#region 2. PIPELINE DE SOLICITUDES (MIDDLEWARE)

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bicicletería Rodríguez v1"));
}

app.UseCors("AllowAll");
//app.UseHttpsRedirection(); //descomentar para pasar a producción con SSL


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

#endregion

app.Run();