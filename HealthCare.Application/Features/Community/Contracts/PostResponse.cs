namespace HealthCare.Application.Features.Community.Contracts;

public record PostResponse(
    Guid Id,
    string Title,
    string Content,
    string? AttachmentUrl,
    bool IsPublished,
    bool IsContainsMedia,
    Guid DoctorId,
    string? DoctorName,   // doctor name and picture are nullable because i use the same response in GetDoctorPosts 
    string? DoctorProfilePicture,
    Guid SpecialtyId,
    string SpecialtyName,
    DateTime Date
);
