using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Nurses.Contracts;

public record UpdateNursePricingRequest(
    decimal HomeVisitFee,
    decimal HourPrice
);
