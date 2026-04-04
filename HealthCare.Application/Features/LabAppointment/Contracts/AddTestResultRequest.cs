using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.LabAppointment.Contracts;

public record AddTestResultRequest(
    decimal Value,
    string Summary,
    IFormFile? ResultFile
);
