using HealthCare.Application.Common.Result;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace HealthCare.Application.Features.Payment.Commands.PaymentCallback;

public record PaymentCallbackCommand(JsonElement Payload, string Signature) : IRequest<Result>;
