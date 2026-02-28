using HealthCare.Application.Common.Result;

namespace HealthCare.Application.Errors;

public static class UserErrors
{
    public static readonly Error InvalidCradentials =
        new("User.InvalidCreadintials", "Invalid Email or Password", 400);
    public static readonly Error NotFound =
        new("User.NotFound", "User is Not Found", 400);
    public static readonly Error InvalidToken =
        new("User.InvalidToken", "Invalid Auth Token", 400);
    public static readonly Error DublicatedEmail =
        new("User.DublicatedEmail", "Email is already exists", 409);
    public static readonly Error EmailNotConfirmed =
        new("User.EmailNotConfirmed", "Email is Not Confirmed", 401);
    public static readonly Error EmailConfirmed =
        new("User.EmailConfirmed", "Email is already Confirmed", 400);
    public static readonly Error InvalidCode =
        new("User.InvalidCode", "Invalid code", 401);
    public static readonly Error DisabledUser =
        new("User.DisabledUser", "Disabled user, please contact your administrator", 401);
    public static readonly Error LockedUser =
        new("User.LockedUser", "Locked user, please contact your administrator", 401);

    public static readonly Error InvalidRefreshToken =
        new("User.InvalidRefreshToken", "refresh token is invalid or expierd", 404);
}
