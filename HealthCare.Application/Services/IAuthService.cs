using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.Auth.Commands.RegisterPatient;
using HealthCare.Application.Features.Auth.Contracts;
using HealthCare.Application.Interfaces.Repositories;
using HealthCare.Domain.Users;

public interface IAuthService 
{
    Task<Result<AuthResponse>> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<Result<RegisterResponse>> RegisterPatientAsync(RegisterPatientCommand request, CancellationToken cancellationToken = default);
    Task<Result> ConfirmEmailAsync(string userId, string token, CancellationToken cancellationToken = default);
    Task<Result> ResendConfirmationEmailAsync(string email, string callbackUrl, CancellationToken cancellationToken = default);
    Task<Result> ForgotPasswordAsync(string email, string callbackUrl, CancellationToken cancellationToken = default);
    Task<Result> ResetPasswordAsync(string userId, string token, string newPassword, CancellationToken cancellationToken = default);
    Task<Result> ChangePasswordAsync(string userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);
    Task<bool> IsUserExist(string email, CancellationToken cancellationToken = default);
}