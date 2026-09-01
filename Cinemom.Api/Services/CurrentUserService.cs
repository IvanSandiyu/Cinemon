using Cinemon.Application.Interfaces;
using System.Security.Claims;

namespace Cinemon.Api.Services
{
    public sealed class CurrentUserService(
    IHttpContextAccessor httpContextAccessor) : ICurrentUserService
    {
        public int UserId
        {
            get
            {
                var userId = httpContextAccessor.HttpContext?
                    .User
                    .FindFirstValue(ClaimTypes.NameIdentifier);

                if (!int.TryParse(userId, out var id))
                    throw new UnauthorizedAccessException();

                return id;
            }
        }

        public string Role =>
            httpContextAccessor.HttpContext?
                .User
                .FindFirstValue(ClaimTypes.Role)
            ?? throw new UnauthorizedAccessException();
    }
}
