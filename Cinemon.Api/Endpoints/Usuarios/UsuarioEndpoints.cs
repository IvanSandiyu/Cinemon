using Cinemon.Application.Usuarios.ActivarUsuarios.Commands;
using Cinemon.Application.Usuarios.CrearUsuarios.Commands;
using Cinemon.Application.Usuarios.Login;
using Cinemon.Application.Usuarios.ObtenerUsuarios.Queries;
using MediatR;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using System.Threading;

namespace Cinemon.Api.Endpoints.Usuarios
{
    public static class UsuarioEndpoints
    {
        public static IEndpointRouteBuilder MapUsuarioEndpoints(
            this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/usuarios")
                .WithTags("Usuarios");

            group.MapGet("/", ObtenerUsuarios).RequireAuthorization(policy =>policy.RequireRole("Admin"));
            
            group.MapGet("/{id:int}", ObtenerUsuario).RequireAuthorization(policy => policy.RequireRole("Admin","Cliente"));
            group.MapPatch("/{id:int}/activar", ActivarUsuario).RequireAuthorization(policy => policy.RequireRole("Admin"));
            group.MapPatch("/{id:int}/desactivar", DesactivarUsuario).RequireAuthorization(policy => policy.RequireRole("Admin"));

            group.MapPost("/", CrearUsuario);
            group.MapPost("/admin", CrearUsuarioAdmin).RequireAuthorization(policy =>policy.RequireRole("Admin"));
            //group.MapGet("/{id}",VerUsuario).RequireAuthorization(policy =>policy.RequireRole("Admin"));

            group.MapPost("/login", Login);

            //group.MapPost("/admin").RequireAuthorization(policy =>policy.RequireRole("Admin"));
            

            return app;
        }

        private static async Task<IResult> DesactivarUsuario(int id,ISender sender,CancellationToken cancellationToken)
        {
            await sender.Send(new DesactivarUsuarioCommand(id),cancellationToken);

            return Results.NoContent();
        }

        private static async Task<IResult> ActivarUsuario(int id,ISender sender,CancellationToken cancellationToken)
        {
            await sender.Send(new ActivarUsuarioCommand(id),cancellationToken);

            return Results.NoContent();
        }
        private static async Task<IResult> ObtenerUsuario(int id,ISender sender,CancellationToken cancellationToken)
        {
            var usuario = await sender.Send(new ObtenerUsuarioPorIdQuery(id),cancellationToken);

            if (usuario is null)
                return Results.NotFound();

            return Results.Ok(new UsuarioDto(
                usuario.Id,
                usuario.NombreApellido,
                usuario.Email,
                usuario.Activo,
                usuario.Rol.ToString()));
        }
        private static async Task<IResult> ObtenerUsuarios(ISender sender,CancellationToken cancellationToken)
        {
            var usuarios = await sender.Send(new ObtenerUsuariosQuery(),cancellationToken);

            return Results.Ok(usuarios);
        }

        private static async Task<IResult> CrearUsuario(CrearUsuarioCommand command,ISender sender,CancellationToken cancellationToken)
        {
            var usuarioId = await sender.Send(command,cancellationToken);

            return Results.Created($"/api/usuarios/{usuarioId}",new{
                    Id = usuarioId
                });
        }
        private static async Task<IResult> Login(LoginCommand command,ISender sender,CancellationToken cancellationToken)
        {
            var response = await sender.Send(command,cancellationToken);

            return Results.Ok(response);
        }

        private static async Task<IResult> CrearUsuarioAdmin(CrearUsuarioAdminCommand command,ISender sender,CancellationToken cancellationToken)
        {
            var usuarioId = await sender.Send(
                command,
                cancellationToken);

            return Results.Created(
                $"/api/usuarios/{usuarioId}",
                new
                {
                    Id = usuarioId
                });
        }

        private sealed record UsuarioDto(
            int Id,
            string NombreApellido,
            string Email,
            bool Activo,
            string Rol);
    }
}
