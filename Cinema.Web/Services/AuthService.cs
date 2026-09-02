namespace Cinemon.Web.Services
{
    public sealed class AuthService
    {
        private readonly HttpClient _httpClient;

        public string? Token { get; private set; }

        public bool EstaAutenticado =>
            !string.IsNullOrWhiteSpace(Token);

        public AuthService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("CinemonApi");
        }

        public async Task<bool> LoginAsync(
            string email,
            string password,
            CancellationToken cancellationToken = default)
        {
            var request = new
            {
                email,
                password
            };

            var response = await _httpClient.PostAsJsonAsync(
                "api/auth/login",
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

            return true;
        }

        public void Logout()
        {
            Token = null;
        }

        private sealed record LoginResponse(
            string Token);
    }
}
