using Cinemon.Application.DTOs.Candy;
using Cinemon.Domain.Enums;
using MediatR;
using System.Collections.Generic;

namespace Cinemon.Application.Candy.Productos.Commands.ActualizarProductoCandy
{
    public sealed record ActualizarProductoCandyCommand(
        int Id,
        string Nombre,
        string? Descripcion,
        decimal Precio,
        CategoriaCandy Categoria,
        IReadOnlyCollection<ComponenteComboRequest>? Componentes) : IRequest<Unit>;
}
