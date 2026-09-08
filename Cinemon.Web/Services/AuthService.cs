using Microsoft.JSInterop;
using System.Text.Json;

namespace Cinemon.Web.Services
{
    public sealed class AuthService
    {
        private const string StorageKey = "cinemon:auth";

        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _js;

        public string? Token { get; private set; }
        public int? UsuarioId { get; private set; }
        public string? NombreApellido { get; private set; }
        public int? Rol { get; private set; }

        public bool EstaAutenticado =>
            !string.IsNullOrWhiteSpace(Token);

        public AuthService(
            IHttpClientFactory httpClientFactory,
            IJSRuntime js)
        {
            _httpClient = httpClientFactory.CreateClient("CinemonApi");
            _js = js;
        }

        public async Task<bool> CargarAsync(
            CancellationToken cancellationToken = default)
        {
            try {
                var json = await _js.InvokeAsync<string>(
                    "localStorage.getItem",
                    cancellationToken,
                    StorageKey);

                if (string.IsNullOrWhiteSpace(json))
                    return false;

                var guardado = JsonSerializer.Deserialize<SesionGuardada>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (guardado is null ||
                    string.IsNullOrWhiteSpace(guardado.Token)) {
                    return false;
                }

                Token = guardado.Token;
                UsuarioId = guardado.UsuarioId;
                NombreApellido = guardado.NombreApellido;
                Rol = guardado.Rol;

                return true;
            } catch {
                return false;
            }
        }

        public async Task<bool> LoginAsync(
            string email,
            string password,
            bool recordar = true,
            CancellationToken cancellationToken = default)
        {
            var request = new
            {
                email,
                password
            };

            var response = await _httpClient.PostAsJsonAsync(
                "api/usuarios/login",
                request,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
                return false;

            var result = await response.Content
                .ReadFromJsonAsync<LoginResponse>(
                    cancellationToken);

            if (result is null || string.IsNullOrWhiteSpace(result.Token))
                return false;

            Token = result.Token;
            UsuarioId = result.UsuarioId;
            NombreApellido = result.NombreApellido;
            Rol = result.Rol;

            if (recordar) {
                await GuardarSesionAsync(cancellationToken);
            }

            return true;
        }

        public async Task<string?> RegistrarAsync(
            string nombreApellido,
            string email,
            string password,
            CancellationToken cancellationToken = default)
        {
            try {
                var request = new
                {
                    NombreApellido = nombreApellido,
                    Email = email,
                    Password = password
                };

                var response = await _httpClient.PostAsJsonAsync(
                    "api/usuarios",
                    request,
                    cancellationToken);

                if (response.IsSuccessStatusCode)
                    return null;

                return await LeerMensajeErrorAsync(
                    response,
                    cancellationToken,
                    "No se pudo crear la cuenta. Verificá los datos ingresados.");
            } catch {
                return "No se pudo conectar con el servidor.";
            }
        }

        private static async Task<string> LeerMensajeErrorAsync(
            HttpResponseMessage response,
            CancellationToken cancellationToken,
            string fallback)
        {
            try {
                var contenido = await response.Content
                    .ReadAsStringAsync(cancellationToken);

                if (string.IsNullOrWhiteSpace(contenido))
                    return fallback;

                var problema = JsonSerializer.Deserialize<ProblemDetails>(
                    contenido,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (problema is not null &&
                    !string.IsNullOrWhiteSpace(problema.Title)) {
                    return problema.Title;
                }

                if (problema?.Errors is not null) {
                    var mensaje = string.Join(
                        " ",
                        problema.Errors.SelectMany(x => x.Value));

                    if (!string.IsNullOrWhiteSpace(mensaje))
                        return mensaje;
                }
            } catch {
            }

            return fallback;
        }

        public async Task LogoutAsync(
            CancellationToken cancellationToken = default)
        {
            Token = null;
            UsuarioId = null;
            NombreApellido = null;
            Rol = null;

            try {
                await _js.InvokeVoidAsync(
                    "localStorage.removeItem",
                    cancellationToken,
                    StorageKey);
            } catch {
            }
        }

        private async Task GuardarSesionAsync(
            CancellationToken cancellationToken)
        {
            var json = JsonSerializer.Serialize(
                new SesionGuardada(
                    Token,
                    UsuarioId,
                    NombreApellido,
                    Rol));

            try {
                await _js.InvokeVoidAsync(
                    "localStorage.setItem",
                    cancellationToken,
                    StorageKey,
                    json);
            } catch {
            }
        }

        private sealed record SesionGuardada(
            string? Token,
            int? UsuarioId,
            string? NombreApellido,
            int? Rol);

        private sealed record LoginResponse(
            string? Token,
            int? UsuarioId,
            string? NombreApellido,
            int? Rol);

        private sealed record ProblemDetails(
            string? Title,
            IReadOnlyDictionary<string, string[]>? Errors);
    }
}