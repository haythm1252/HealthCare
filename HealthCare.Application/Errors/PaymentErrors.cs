using HealthCare.Application.Common.Result;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Errors;

public static class PaymentErrors
{
    public static readonly Error PaymentProviderFailed =
        new("Payment.ProviderError", "The payment provider returned an error while initializing the transaction.", 400);

    public static readonly Error ParsingFailed =
        new("Payment.ParsingError", "Failed to process the response from the payment provider.", 500);
}
