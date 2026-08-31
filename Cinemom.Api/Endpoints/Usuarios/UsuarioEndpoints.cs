using Cinemon.Application.Usuarios.Commands;
using Cinemon.Application.Usuarios.Login;
using MediatR;

namespace Cinemon.Api.Endpoints.Usuarios
{
    public static class UsuarioEndpoints
    {
        public static IEndpointRouteBuilder MapUsuarioEndpoints(
            this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/usuarios")
                .WithTags("Usuarios");

            group.MapPost("/", CrearUsuario);

            group.MapPost("/login", Login);

            return app;
        }

        private static async Task<IResult> CrearUsuario(
            CrearUsuarioCommand command,
            ISender sender,
            CancellationToken cancellationToken)
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
        private static async Task<IResult> Login(LoginCommand command,ISender sender,CancellationToken cancellationToken)
        {
            var response = await sender.Send(
                command,
                cancellationToken);

            return Results.Ok(response);
        }
    }
}
