using HealthCare.Application.Features.Patients.Contracts;
using HealthCare.Domain.Users;
using Mapster;

namespace HealthCare.Application.Mapping;

public class PatientMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Patient, PatientProfileResponse>()
            .Map(dest => dest.Name, src => src.User.Name)
            .Map(dest => dest.Email, src => src.User.Email)
            .Map(dest => dest.PhoneNumber, src => src.User.PhoneNumber)
            .Map(dest => dest.City, src => src.User.City)
            .Map(dest => dest.Address, src => src.User.Address)
            .Map(dest => dest.AddressUrl, src => src.User.AddressUrl)
            .Map(dest => dest.Gender, src => src.User.Gender);

    }
}
