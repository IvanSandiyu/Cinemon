using Cinemon.Application.Interfaces;
using System.Collections.Generic;

namespace Cinemon.Tests.Fakes
{
    /// <summary>
    /// Simula al usuario autenticado para poder ejecutar los handlers
    /// sin levantar la API ni el pipeline de JWT.
    /// </summary>
    public sealed class FakeCurrentUserService : ICurrentUserService
    {
        public FakeCurrentUserService(int userId,string role = "Cliente")
        {
            UserId = userId;
            Role = role;
        }

        public int UserId { get; }

        public string Role { get; }
    }
}
