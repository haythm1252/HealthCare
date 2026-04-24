using HealthCare.Application.Common.Result;
using HealthCare.Application.Common.Settings;
using HealthCare.Application.Features.Communication.Contracts;
using HealthCare.Application.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace HealthCare.Infrastructure.Services;

public class MeetingService(IOptions<MeetingSettings> meetingSettings,ILogger<MeetingService> logger) : IMeetingService
{
    private readonly MeetingSettings _meetingSettings = meetingSettings.Value;
    private readonly ILogger<MeetingService> _logger = logger;

    public async Task<Result<string>> CreateMeetingAsync(Guid appointmentId, DateTime endDateTime)
    {
        try
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_meetingSettings.ApiKey}");

            var requestBody = new
            {
                endDate = endDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                isLocked = false,
                roomMode = "normal"
            };

            _logger.LogInformation("Attempting to create a Whereby meeting for Appointment: {AppointmentId}", appointmentId);

            var response = await client.PostAsJsonAsync("https://api.whereby.dev/v1/meetings", requestBody);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Whereby API failed with status {StatusCode}: {Error}", response.StatusCode, errorContent);

                return Result.Failure<string>(new Error(
                    "DoctorAppointment.WherebyApiError",
                    $"Failed to create video room, {errorContent}",
                    500));
            }

            var result = await response.Content.ReadFromJsonAsync<JsonElement>();
            if (result.TryGetProperty("roomUrl", out var roomUrlElement))
            {
                string roomUrl = roomUrlElement.GetString()!;
                if(roomUrl is null)
                {
                    _logger.LogWarning("Whereby API returned null 'roomUrl' for Appointment: {AppointmentId}", appointmentId);
                    return Result.Failure<string>(new Error("DoctorAppointment.NullRoomUrl",
                        "The 'roomUrl' property in the response from Whereby was null.",500));
                }

                _logger.LogInformation("Successfully created meeting room for Appointment: {AppointmentId}", appointmentId);
                return Result.Success(roomUrl);
            }
            else
            {
                _logger.LogWarning("Whereby API response missing 'roomUrl' for Appointment: {AppointmentId}", appointmentId);

                return Result.Failure<string>(new Error(
                    "DoctorAppointment.MissingProperty",
                    "The response from Whereby did not contain the 'roomUrl' property.",500));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while creating Whereby meeting for {AppointmentId}", appointmentId);

            return Result.Failure<string>(new Error(
                "DoctorAppointment.InternalException",
                ex.Message,
                500));
        }
    }
}

