using HealthCare.Application.Features.DoctorSlots.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Doctors.Contracts;

public record DoctorScheduleResponse(
    decimal ClinicFee,
    decimal HomeVisitFee,
    decimal OnlineFee,
    bool HomeVisit,
    bool OnlineConsultation,
    IEnumerable<DailySlotsDto> Slots
);
