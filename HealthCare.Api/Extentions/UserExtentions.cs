using System.Security.Claims;

namespace HealthCare.Api.Extentions;

public static class UserExtentions
{
    extension(ClaimsPrincipal user)
    {
        public string? GetUserId()
            => user.FindFirstValue(ClaimTypes.NameIdentifier);
        public string? GetRole()
            => user.FindFirstValue(ClaimTypes.Role);
    }
}
