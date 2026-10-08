using Cinemon.Domain.Enums;
using Cinemon.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinemon.Domain.Entidades.Candy
{
    public class ProductoCandy
    {
        public int Id { get; private set; }

        public string Nombre { get; private set; } = string.Empty;

        public string? Descripcion { get; private set; }

        public decimal Precio { get; private set; }

        public CategoriaCandy Categoria { get; private set; }

        public bool Activo { get; private set; }

        public DateTime FechaCreacion { get; private set; }

        public ICollection<ProductoComboItem> Componentes { get; private set; } = [];

        private ProductoCandy()
        {
        }

        public ProductoCandy(
            string nombre,
            string? descripcion,
            decimal precio,
            CategoriaCandy categoria,
            IEnumerable<ProductoComboItem>? componentes = null)
        {
            Setear(nombre, descripcion, precio, categoria);
            ArmarComponentes(categoria, componentes);

            Activo = true;
            FechaCreacion = DateTime.UtcNow;
        }

        public void Actualizar(
            string nombre,
            string? descripcion,
            decimal precio,
            CategoriaCandy categoria,
            IEnumerable<ProductoComboItem>? componentes = null)
        {
            Setear(nombre, descripcion, precio, categoria);
            ArmarComponentes(categoria, componentes);
        }

        public void Activar() => Activo = true;

        public void Desactivar() => Activo = false;

        /// <summary>
        /// Un combo nunca puede costar más que la suma de sus componentes
        /// comprados por separado. La suma se calcula fuera (con los precios
        /// vigentes en base) y se pasa acá para que la regla viva en dominio.
        /// </summary>
        public void ValidarPrecioCombo(decimal sumaComponentes)
        {
            if (Categoria != CategoriaCandy.Combo)
                return;

            if (Componentes.Count == 0)
                throw new BusinessRuleException("Un combo debe tener al menos un producto.");

            if (Precio > sumaComponentes)
                throw new BusinessRuleException(
                    "El precio del combo no puede superar la suma de sus componentes.");
        }

        private void Setear(
            string nombre,
            string? descripcion,
            decimal precio,
            CategoriaCandy categoria)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new BusinessRuleException("El producto debe tener un nombre.");

            if (nombre.Trim().Length > 100)
                throw new BusinessRuleException("El nombre del producto no puede superar los 100 caracteres.");

            if (precio <= 0)
                throw new BusinessRuleException("El precio del producto debe ser mayor a cero.");

            if (!Enum.IsDefined(categoria))
                throw new BusinessRuleException("La categoría del producto no es válida.");

            if (descripcion is not null && descripcion.Trim().Length > 500)
                throw new BusinessRuleException("La descripción no puede superar los 500 caracteres.");

            Nombre = nombre.Trim();
            Descripcion = descripcion?.Trim();
            Precio = precio;
            Categoria = categoria;
        }

        private void ArmarComponentes(
            CategoriaCandy categoria,
            IEnumerable<ProductoComboItem>? componentes)
        {
            var lista = componentes?.ToList() ?? [];

            Componentes.Clear();

            if (categoria != CategoriaCandy.Combo)
            {
                if (lista.Count > 0)
                    throw new BusinessRuleException("Solo los combos pueden tener componentes.");

                return;
            }

            if (lista.Count == 0)
                throw new BusinessRuleException("Un combo debe tener al menos un producto.");

            if (lista.Any(x => x.Cantidad < 1))
                throw new BusinessRuleException("La cantidad de cada componente debe ser al menos 1.");

            var agrupados = lista
                .GroupBy(x => x.ComponenteProductoId)
                .Select(g => new ProductoComboItem(g.Key, g.Sum(x => x.Cantidad)));

            foreach (var item in agrupados)
                Componentes.Add(item);
        }
    }
}
