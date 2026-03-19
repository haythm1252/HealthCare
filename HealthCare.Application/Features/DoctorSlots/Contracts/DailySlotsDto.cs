using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.DoctorSlots.Contracts;

public record DailySlotsDto(
    DateOnly Date,
    string Day,
    IEnumerable<SlotDto> Slots
);
