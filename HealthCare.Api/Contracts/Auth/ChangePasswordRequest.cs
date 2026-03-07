namespace HealthCare.Api.Contracts.Auth;

public record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword,
    string ConfirmPassword
);
