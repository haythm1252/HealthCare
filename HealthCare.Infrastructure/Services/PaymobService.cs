using HealthCare.Application.Common.Result;
using HealthCare.Application.Common.Settings;
using HealthCare.Application.Errors;
using HealthCare.Application.Services;
using HealthCare.Domain.Entities;
using HealthCare.Domain.Users;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace HealthCare.Infrastructure.Services;

public class PaymobService(IOptions<PaymobSettings> paymobSettings) : IPaymobService
{
    private readonly PaymobSettings _paymobSettings = paymobSettings.Value;

    // We return the URL and the Intention ID so the Handler can update the DB
    public async Task<Result<(string CheckoutUrl, string IntentionId)>> ProcessPaymentAsync(DoctorAppointment appointment, Patient patient)
    {
        var amountCents = (int)(appointment.Fee * 100);
        var httpClient = new HttpClient();

        var payload = new
        {
            amount = amountCents,
            currency = _paymobSettings.Currency,
            payment_methods = new[] { int.Parse(_paymobSettings.IntegrationId) },
            billing_data = new
            {
                first_name = patient.User.Name,
                last_name = "patient",
                email = patient.User.Email,
                phone_number = patient.User.PhoneNumber,
                street = "NA",
                building = "NA",
                floor = "NA",
                apartment = "NA",
                city = patient.User.City,
                state = patient.User.City,
                country = "EG"
            },
            special_reference = appointment.Id.ToString(),
            merchant_order_id = appointment.Id.ToString()
        };

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://accept.paymob.com/v1/intention/");
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Token", _paymobSettings.SecretKey);
        requestMessage.Content = JsonContent.Create(payload);

        var response = await httpClient.SendAsync(requestMessage);

        if (!response.IsSuccessStatusCode)
            return Result.Failure<(string CheckoutUrl, string IntentionId)>(PaymentErrors.PaymentProviderFailed);

        var responseContent = await response.Content.ReadAsStringAsync();

        try
        {
            using var jsonDoc = JsonDocument.Parse(responseContent);
            var clientSecret = jsonDoc.RootElement.GetProperty("client_secret").GetString();
            var intentionId = jsonDoc.RootElement.GetProperty("id").GetString();

            var checkoutUrl = $"https://accept.paymob.com/unifiedcheckout/?publicKey={_paymobSettings.PublicKey}&clientSecret={clientSecret}";

            return Result.Success((checkoutUrl, intentionId!));
        }
        catch
        {
            return Result.Failure<(string checkoutUrl, string IntentionId)>(PaymentErrors.ParsingFailed);
        }
    }

    public bool IsValidSignature(JsonElement payload, string receivedHmac)
    {

        string secret = _paymobSettings.HmacSecret;

        if (!payload.TryGetProperty("obj", out var obj))
            return false;

        string[] fields =
        [
            "amount_cents", "created_at", "currency", "error_occured", "has_parent_transaction",
            "id", "integration_id", "is_3d_secure", "is_auth", "is_capture", "is_refunded",
            "is_standalone_payment", "is_voided", "order.id", "owner", "pending",
            "source_data.pan", "source_data.sub_type", "source_data.type", "success"
        ];

        var concatenated = new StringBuilder();
        foreach (var field in fields)
        {
            string[] parts = field.Split('.');
            JsonElement current = obj;
            bool found = true;
            foreach (var part in parts)
            {
                if (current.ValueKind == JsonValueKind.Object && current.TryGetProperty(part, out var next))
                    current = next;
                else
                {
                    found = false;
                    break;
                }
            }

            if (!found || current.ValueKind == JsonValueKind.Null)
            {
                concatenated.Append(""); // Use empty string for missing/null fields
            }
            else if (current.ValueKind == JsonValueKind.True || current.ValueKind == JsonValueKind.False)
            {
                concatenated.Append(current.GetBoolean() ? "true" : "false"); // Lowercase boolean
            }
            else
            {
                concatenated.Append(current.ToString());
            }
        }
        var calculatedHmac = ComputeHmacSHA512(concatenated.ToString(), secret);
        return receivedHmac.Equals(calculatedHmac, StringComparison.OrdinalIgnoreCase);
    }

    private string ComputeHmacSHA512(string data, string key)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var dataBytes = Encoding.UTF8.GetBytes(data);

        using var hmac = new HMACSHA512(keyBytes);
        var hashBytes = hmac.ComputeHash(dataBytes);

        return Convert.ToHexString(hashBytes).ToLower();
    }


}