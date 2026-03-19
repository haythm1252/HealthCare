using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.DoctorSlots.Contracts;

public record GenerateDoctorSlotsRequest(
    DateOnly StartDate,
    DateOnly? EndDate,
    TimeOnly StartTime,
    TimeOnly? EndTime,
    int ConsultationDurationInminutes = 5
);