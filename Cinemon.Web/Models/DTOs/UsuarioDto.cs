namespace Cinemon.Web.Models.DTOs
{
    public sealed record UsuarioDto(
        int Id,
        string NombreApellido,
        string Email,
        bool Activo,
        string Rol);
}