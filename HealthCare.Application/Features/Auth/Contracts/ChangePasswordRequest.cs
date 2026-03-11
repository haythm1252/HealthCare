namespace HealthCare.Application.Features.Auth.Contracts;

public record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword
);
