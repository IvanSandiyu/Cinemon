using Microsoft.JSInterop;

namespace Cinemon.Web.Services
{
    public sealed class FavoritosService
    {
        private readonly IJSRuntime _js;
        private HashSet<int> _ids = [];

        public FavoritosService(IJSRuntime js) => _js = js;

        public bool EsFavorito(int id) => _ids.Contains(id);

        public async Task CargarAsync()
        {
            try
            {
                var raw = await _js.InvokeAsync<string>("cinemonFavoritos.obtener");
                _ids = Parsear(raw);
            }
            catch
            {
                _ids = [];
            }
        }

        public async Task AlternarAsync(int id)
        {
            if (!_ids.Add(id))
                _ids.Remove(id);

            await GuardarAsync();
        }

        private async Task GuardarAsync()
        {
            try
            {
                await _js.InvokeVoidAsync(
                    "cinemonFavoritos.guardar",
                    string.Join(',', _ids));
            }
            catch
            {
            }
        }

        private static HashSet<int> Parsear(string? raw)
        {
            var set = new HashSet<int>();

            if (string.IsNullOrWhiteSpace(raw))
                return set;

            foreach (var parte in raw.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                if (int.TryParse(parte, out var id))
                    set.Add(id);
            }

            return set;
        }
    }
}