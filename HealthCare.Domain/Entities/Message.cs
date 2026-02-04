using HealthCare.Domain.Enums;
using HealthCare.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Domain.Entities;

public sealed class Message : BaseEntity
{
    public string Content { get; set; } = string.Empty;
    public string? AttachmentUrl { get; set; }
    public string? AttachmentPublicId { get; set; }
    public ContentType ContentType { get; set; }

    public Guid ChatId { get; set; }
    public Chat Chat { get; set; } = default!;
    public string SenderId { get; set; } = string.Empty;
    public ApplicationUser Sender { get; set; } = default!;
}
