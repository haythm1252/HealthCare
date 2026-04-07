using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.DoctorAppointments.Contracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.DoctorAppointments.Queries.PaymentStatus;

public record DoctorAppointmentPaymentStatusQuery(string UserId, Guid AppointmentId) : IRequest<Result<AppointmentPaymentStatusResponse>>;
