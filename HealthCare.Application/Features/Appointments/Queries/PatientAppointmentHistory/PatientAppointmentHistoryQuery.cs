using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.Appointments.Contracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Appointments.Queries.PatientAppointmentHistory;

public record PatientAppointmentHistoryQuery(string UserId) : IRequest<Result<PatientAppointmentHistoryResponse>>;
