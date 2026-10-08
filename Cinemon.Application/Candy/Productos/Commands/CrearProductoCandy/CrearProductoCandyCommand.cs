using Cinemon.Application.DTOs.Candy;
using Cinemon.Domain.Enums;
using MediatR;
using System.Collections.Generic;

namespace Cinemon.Application.Candy.Productos.Commands.CrearProductoCandy
{
    public sealed record CrearProductoCandyCommand(
        string Nombre,
        string? Descripcion,
        decimal Precio,
        CategoriaCandy Categoria,
        IReadOnlyCollection<ComponenteComboRequest>? Componentes) : IRequest<int>;
}
