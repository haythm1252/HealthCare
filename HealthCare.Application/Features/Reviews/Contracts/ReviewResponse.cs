using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Reviews.Contracts;

public record ReviewResponse(
    Guid Id, 
    string PatientName,
    int Rating,
    string? Comment,
    DateTime Date,
    bool IsUpdated
);
