using Cinemon.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Cinemon.Domain.Entidades.Usuarios
{
    public class Usuario
    {
        public int Id { get; set; }
        public string NombreApellido { get; set; }
        public string Email { get; set; }
        public bool Activo { get; set; }
        public Rol Rol { get; set; }

        public Usuario(
       string nombre,
       string apellido,
       string email,
       Rol rol)
        {
            NombreApellido = nombre;
            Email = email;
            Rol = rol;
            Activo = true;
        }

        public void Activar()
        {
            Activo = true;
        }

        public void Desactivar()
        {
            Activo = false;
        }

    }
}
