namespace Cinemon.Web.Models.DTOs
{
    public sealed record UsuarioAdminDto(
        int Id,
        string NombreApellido,
        string Email,
        bool Activo,
        int Rol);
}