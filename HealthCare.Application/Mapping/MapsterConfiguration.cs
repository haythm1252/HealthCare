using HealthCare.Application.Features.Auth.Commands.RegisterPatient;
using HealthCare.Application.Features.Auth.Contracts;
using HealthCare.Domain.Enums;
using HealthCare.Domain.Users;
using Mapster;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Mapping;

public class MapsterConfiguration : IRegister
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

    }
}
