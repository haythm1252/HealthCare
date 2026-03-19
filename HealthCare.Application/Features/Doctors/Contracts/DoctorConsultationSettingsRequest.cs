using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Doctors.Contracts;

public record DoctorConsultationSettingsRequest(
    decimal ClinicFee,
    decimal HomeFee,
    decimal OnlineFee,
    bool AllowHomeVisit,
    bool AllowOnlineConsultation
);
