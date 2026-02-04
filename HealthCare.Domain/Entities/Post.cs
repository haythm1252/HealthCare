using HealthCare.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Domain.Entities;

public sealed class Post : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string ProfilePictureUrl { get; set; } = string.Empty;
    public string ProfilePicturePublicId { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
    public bool IsContainsMedia { get; set; }

    public Guid DoctorId { get; set; }
    public Doctor Doctor { get; set; } = default!;
    public Guid SpecialtyId { get; set; }
    public Specialty Specialty { get; set; } = default!;

}
