using Cinemon.Application.Usuarios;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Infrastructure.Authentication
{
    public class PasswordService : IPasswordService
    {
        private readonly PasswordHasher<object> _passwordHasher = new();

        public string HashPassword(string password)
        {
            return _passwordHasher.HashPassword(
                new object(),
                password);
        }

        public bool VerifyPassword(
            string password,
            string passwordHash)
        {
            var result = _passwordHasher.VerifyHashedPassword(
                new object(),
                passwordHash,
                password);

            return result == PasswordVerificationResult.Success ||
                   result == PasswordVerificationResult.SuccessRehashNeeded;
        }
    }
}
