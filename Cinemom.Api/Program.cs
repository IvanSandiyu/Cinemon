using Cinemon.Api.Endpoints;
using Cinemon.Api.Endpoints.Butacas;
using Cinemon.Api.Endpoints.Funciones;
using Cinemon.Api.Endpoints.Generos;
using Cinemon.Api.Endpoints.Peliculas;
using Cinemon.Api.Endpoints.Reservas;
using Cinemon.Api.Endpoints.Salas;
using Cinemon.Api.Endpoints.Usuarios;
using Cinemon.Api.Middleware;
using Cinemon.Api.Services;
using Cinemon.Application;
using Cinemon.Application.Interfaces;
using Cinemon.Infrastructure;
using Cinemon.Infrastructure.Seed;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

//Convierte los enums a string
//builder.Services.ConfigureHttpJsonOptions(options =>
//    options.SerializerOptions.Converters.Add(
//        new JsonStringEnumConverter()));

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Application
builder.Services.AddApplication();

// Infrastructure
builder.Services.AddInfrastructure(
    builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingres� el JWT obtenido mediante /api/usuarios/login."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT Key no configurada.");

var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)),

            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = true,
            ValidAudience = jwtAudience,

            ValidateLifetime = true,

            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();


app.MapPeliculaEndpoint();
app.MapSalaEndpoints();
app.MapButacaEndpoints();
app.MapFuncionEndpoints();
app.MapReservaEndpoints();
app.MapUsuarioEndpoints();
app.MapTmdbEndpoints();
app.MapGeneroEndpoints();

using (var scope = app.Services.CreateScope()) {
    var context = scope.ServiceProvider
        .GetRequiredService<CinemonDbContext>();

    await CinemonDbContextSeed.SeedAsync(context);
}

app.Run();
