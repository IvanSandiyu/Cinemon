namespace Cinemon.Web.Models.DTOs
{
    public sealed record ComponenteComboDto(
        int ProductoId,
        string Nombre,
        int Cantidad,
        decimal PrecioUnitario);

    public sealed record ProductoCandyDto(
        int Id,
        string Nombre,
        string? Descripcion,
        decimal Precio,
        int Categoria,
        bool Activo,
        IReadOnlyCollection<ComponenteComboDto> Componentes);

    public sealed record ComponenteComboRequest(
        int ProductoId,
        int Cantidad);

    public sealed record CrearProductoCandyRequest(
        string Nombre,
        string? Descripcion,
        decimal Precio,
        int Categoria,
        IReadOnlyCollection<ComponenteComboRequest> Componentes);

    public sealed record ActualizarProductoCandyRequest(
        int Id,
        string Nombre,
        string? Descripcion,
        decimal Precio,
        int Categoria,
        IReadOnlyCollection<ComponenteComboRequest> Componentes);

    public sealed record PedidoCandyItemDto(
        int ProductoId,
        string NombreProducto,
        decimal PrecioUnitario,
        int Cantidad,
        decimal Subtotal);

    public sealed record PedidoCandyDto(
        int Id,
        DateTime FechaPedido,
        decimal Total,
        IReadOnlyCollection<PedidoCandyItemDto> Items);

    public sealed record CrearPedidoCandyItemRequest(
        int ProductoId,
        int Cantidad);

    public sealed record CrearPedidoCandyRequest(
        IReadOnlyCollection<CrearPedidoCandyItemRequest> Items);

    public sealed record CarritoItemDto(
        int ProductoId,
        string Nombre,
        string? Descripcion,
        decimal PrecioUnitario,
        int Categoria,
        int Cantidad);
}
