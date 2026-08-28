using Cinemon.Api.Endpoints.Butacas;
using Cinemon.Api.Endpoints.Funciones;
using Cinemon.Api.Endpoints.Peliculas;
using Cinemon.Api.Endpoints.Reservas;
using Cinemon.Api.Endpoints.Salas;
using Cinemon.Api.Middleware;
using Cinemon.Application;
using Cinemon.Infrastructure;
using Cinemon.Infrastructure.Seed;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Application
builder.Services.AddApplication();

// Infrastructure
builder.Services.AddInfrastructure(
    builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.MapPeliculaEndpoint();
app.MapSalaEndpoints();
app.MapButacaEndpoints();
app.MapFuncionEndpoints();
app.MapReservaEndpoints();

using (var scope = app.Services.CreateScope()) {
    var context = scope.ServiceProvider
        .GetRequiredService<CinemonDbContext>();

    await CinemonDbContextSeed.SeedAsync(context);
}

app.Run();
