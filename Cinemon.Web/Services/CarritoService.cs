using Cinemon.Web.Models.DTOs;
using Microsoft.JSInterop;

namespace Cinemon.Web.Services
{
    public sealed class CarritoService
    {
        private readonly IJSRuntime _jsRuntime;

        private List<CarritoItemDto>? _items;

        public CarritoService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public event Action? Cambio;

        public IReadOnlyList<CarritoItemDto> Items => _items ?? [];

        public int CantidadTotal => Items.Sum(x => x.Cantidad);

        public decimal Total => Items.Sum(x => x.PrecioUnitario * x.Cantidad);

        public async Task CargarAsync()
        {
            if (_items is not null)
                return;

            try {
                var items = await _jsRuntime.InvokeAsync<List<CarritoItemDto>>(
                    "cinemonCarrito.leer");

                _items = items ?? [];
            } catch {
                _items = [];
            }
        }

        public async Task AgregarAsync(CarritoItemDto item)
        {
            await CargarAsync();

            var posicion = _items!.FindIndex(x => x.ProductoId == item.ProductoId);

            if (posicion < 0)
                _items.Add(item);
            else
                _items[posicion] = _items[posicion] with { Cantidad = _items[posicion].Cantidad + item.Cantidad };

            await PersistirAsync();
        }

        public async Task CambiarCantidadAsync(int productoId, int cantidad)
        {
            await CargarAsync();

            var posicion = _items!.FindIndex(x => x.ProductoId == productoId);

            if (posicion < 0)
                return;

            if (cantidad < 1)
            {
                _items.RemoveAt(posicion);
            }
            else
            {
                _items[posicion] = _items[posicion] with { Cantidad = cantidad };
            }

            await PersistirAsync();
        }

        public async Task QuitarAsync(int productoId)
        {
            await CargarAsync();

            _items!.RemoveAll(x => x.ProductoId == productoId);

            await PersistirAsync();
        }

        public async Task VaciarAsync()
        {
            _items = [];

            try {
                await _jsRuntime.InvokeVoidAsync("cinemonCarrito.vaciar");
            } catch {
            }

            Cambio?.Invoke();
        }

        private async Task PersistirAsync()
        {
            try {
                await _jsRuntime.InvokeVoidAsync("cinemonCarrito.guardar", _items);
            } catch {
            }

            Cambio?.Invoke();
        }
    }
}
