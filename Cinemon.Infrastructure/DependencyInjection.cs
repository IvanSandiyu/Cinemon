using Cinemon.Application.Abstractions;
using Cinemon.Application.Interfaces;
using Cinemon.Application.Usuarios;
using Cinemon.Application.Usuarios.Interfaces;
using Cinemon.Infrastructure.Authentication;
using Cinemon.Infrastructure.ExternalServices.Tmdb;
using Cinemon.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;


namespace Cinemon.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddDbContext<CinemonDbContext>(options => options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));
            services.Configure<TmdbOptions>(configuration.GetSection(TmdbOptions.SectionName));

            services.AddHttpClient<ITmdbService, TmdbService>((serviceProvider, client) =>
            {
                var options = serviceProvider
                .GetRequiredService<IOptions<TmdbOptions>>().Value;

                client.BaseAddress = new Uri(options.BaseUrl);

                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(
                        "Bearer",
                        options.AccessToken);

                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue(
                        "application/json"));
            });

            services.AddScoped<IPeliculaRepository, PeliculaRepository>();
            services.AddScoped<IGeneroRepository, GeneroRepository>();
            services.AddScoped<ISalaRepository, SalaRepository>();
            services.AddScoped<IButacaRepository, ButacaRepository>();
            services.AddScoped<IFuncionRepository, FuncionRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IReservaRepository, ReservaRepository>();
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<ITokenService, JwtTokenService>();

           


            return services;
        }
    }
}
