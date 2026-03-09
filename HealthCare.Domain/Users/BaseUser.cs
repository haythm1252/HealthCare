using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Domain.Users;

public class BaseUser
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public DateTime? LastModified { get; set; }
}
