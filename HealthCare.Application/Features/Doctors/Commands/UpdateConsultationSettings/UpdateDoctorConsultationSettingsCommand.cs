using HealthCare.Application.Common.Result;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Doctors.Commands.UpdateConsultationSettings;

public record UpdateDoctorConsultationSettingsCommand(
    string UserId,
    decimal ClinicFee,
    decimal HomeFee,
    decimal OnlineFee,
    bool AllowHomeVisit,
    bool AllowOnlineConsultation
) : IRequest<Result>;
