using HealthCare.Application.Features.Labs.Commands.UpdateSchedule;
using HealthCare.Application.Features.Labs.Contracts;
using HealthCare.Application.Features.LabTest.Contracts;
using HealthCare.Domain.Entities;
using HealthCare.Domain.Users;
using Mapster;

namespace HealthCare.Application.Mapping;

public class LabMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Lab, LabProfileResponse>()
            .Map(dest => dest.Name, src => src.User.Name)
            .Map(dest => dest.Email, src => src.User.Email)
            .Map(dest => dest.PhoneNumber, src => src.User.PhoneNumber)
            .Map(dest => dest.City, src => src.User.City)
            .Map(dest => dest.Address, src => src.User.Address)
            .Map(dest => dest.AddressUrl, src => src.User.AddressUrl);

        config.NewConfig<UpdateLabScheduleCommand, Lab>()
            .Ignore(dest => dest.UserId)
            .Map(dest => dest.WorkingDays.IsSaturdayOpen, src => src.IsSaturdayOpen)
            .Map(dest => dest.WorkingDays.IsSundayOpen, src => src.IsSundayOpen)
            .Map(dest => dest.WorkingDays.IsMondayOpen, src => src.IsMondayOpen)
            .Map(dest => dest.WorkingDays.IsThursdayOpen, src => src.IsThursdayOpen)
            .Map(dest => dest.WorkingDays.IsWednesdayOpen, src => src.IsWednesdayOpen)
            .Map(dest => dest.WorkingDays.IsTuesdayOpen, src => src.IsTuesdayOpen)
            .Map(dest => dest.WorkingDays.IsFridayOpen, src => src.IsFridayOpen)
            .Map(dest => dest.LastModified, src => DateTime.UtcNow);

        config.NewConfig<Lab, LabScheduleResponse>()
            .Map(dest => dest.IsSaturdayOpen, src => src.WorkingDays.IsSaturdayOpen)
            .Map(dest => dest.IsSundayOpen, src => src.WorkingDays.IsSundayOpen)
            .Map(dest => dest.IsMondayOpen, src => src.WorkingDays.IsMondayOpen)
            .Map(dest => dest.IsTuesdayOpen, src => src.WorkingDays.IsTuesdayOpen)
            .Map(dest => dest.IsWednesdayOpen, src => src.WorkingDays.IsWednesdayOpen)
            .Map(dest => dest.IsThursdayOpen, src => src.WorkingDays.IsThursdayOpen)
            .Map(dest => dest.IsFridayOpen, src => src.WorkingDays.IsFridayOpen);


        config.NewConfig<LabTest, LabTestResponse>()
            .Map(dest => dest.Name, src => src.Test.Name)
            .Map(dest => dest.Description, src => src.Test.Description)
            .Map(dest => dest.PreRequisites, src => src.Test.PreRequisites);

    }
}