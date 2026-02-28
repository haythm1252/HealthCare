using System.Security.Claims;

namespace HealthCare.Api.Extentions;

public static class UserExtentions
{
    extension(ClaimsPrincipal user)
    {
        public string? GetUserId()
        {
            return user.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
