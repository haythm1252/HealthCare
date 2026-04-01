using HealthCare.Application.Common.Result;
using HealthCare.Application.Common.Settings;
using HealthCare.Domain.Entities;
using HealthCare.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace HealthCare.Application.Services;

public interface IPaymobService
{
    Task<Result<(string CheckoutUrl, string IntentionId)>> ProcessPaymentAsync(DoctorAppointment appointment, Patient patient);
    bool IsValidSignature(JsonElement payload, string receivedHmac);
}
