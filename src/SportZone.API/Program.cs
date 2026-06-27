using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SportZone.API.Middleware;
using SportZone.Application;
using SportZone.Application.Common.Exceptions;
using SportZone.Application.Interfaces.Servicios;
using SportZone.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger con soporte para JWT: agrega el botón "Authorize" donde pegas el token
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Pega el token aquí, SIN escribir la palabra 'Bearer' (Swagger la agrega sola)"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// Permite que el frontend (Vite, en otro puerto/origen) consuma esta API desde el navegador.
// Sin esto, el navegador bloquea la petición aunque la API responda 200 OK.
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendDev", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",
                "http://localhost:5174",
                "http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Le dice a ASP.NET Core CÓMO validar un JWT: con qué clave fue firmado, quién lo emitió,
// para quién es válido, y que rechace tokens vencidos.
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!))
    };
});

builder.Services.AddAuthorization();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Siembra roles/usuario admin, catalogo de productos y datos ficticios de compras/ventas.
    // Cada seeder es idempotente: si ya hay datos, no hace nada (ver sus respectivas implementaciones).
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<IUsuarioSeederServicio>().SeedAsync();

    var dataSeeder = scope.ServiceProvider.GetRequiredService<IDataSeederServicio>();
    var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
    try
    {
        var rutaSeed = Path.Combine(env.ContentRootPath, "Common", "Data", "Seeder.json");
        await dataSeeder.CargarDesdeJsonAsync(rutaSeed);
    }
    catch (ValidationException)
    {
        // Ya se cargo en un arranque anterior; SeederController sigue disponible para recargas manuales.
    }

    await scope.ServiceProvider.GetRequiredService<IDatosFicticiosSeederServicio>().SeedAsync();
}

app.UseHttpsRedirection();
app.UseCors("FrontendDev");

// El orden importa: primero se identifica QUIÉN es (Authentication), luego se revisa
// si tiene PERMISO para lo que pide (Authorization). Al revés no tendría sentido.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
