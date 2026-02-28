using HealthCare.Application.Features.Auth.Commands.RegisterPatient;
using HealthCare.Application.Features.Auth.Contracts;
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
            .Map(dest => dest.UserName, src => src.Email);

        config.NewConfig<RegisterPatientCommand, Patient>()
            .Map(dest => dest.DateOfBirth, src => src.DateOfBirth)
            .Map(dest => dest, src => src.Diseases);

        config.NewConfig<DiseasesDto, Patient>();

    }
}
