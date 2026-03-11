using HealthCare.Application.Features.Auth.Commands.RegisterPatient;
using HealthCare.Application.Features.Auth.Contracts;
using HealthCare.Application.Features.Users.Commands.MedicalStaffRegister;
using HealthCare.Domain.Enums;
using HealthCare.Domain.Users;
using Mapster;

namespace HealthCare.Application.Mapping;

public class AuthMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<RegisterPatientCommand, ApplicationUser>()
            .Map(dest => dest.UserName, src => src.Email)
            .Map(dest => dest.Gender, src => Enum.Parse<Gender>(src.Gender, true));

        config.NewConfig<RegisterPatientCommand, Patient>()
            .Map(dest => dest.DateOfBirth, src => src.DateOfBirth)
            .Map(dest => dest, src => src.Diseases);

        config.NewConfig<DiseasesDto, Patient>();

        config.NewConfig<MedicalStaffRegisterCommand, ApplicationUser>()
            .Map(dest => dest.UserName, src => src.Email)
            .Map(dest => dest.Gender, src => ParseGender(src.Gender));
    }

    private static Gender? ParseGender(string? gender)
    {
        if (Enum.TryParse<Gender>(gender, true, out var g))
            return g;
        return null;
    }
}
