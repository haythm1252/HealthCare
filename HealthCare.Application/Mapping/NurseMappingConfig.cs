using HealthCare.Application.Features.Nurses.Commands.UpdatePricing;
using HealthCare.Application.Features.Nurses.Contracts;
using HealthCare.Domain.Users;
using Mapster;

namespace HealthCare.Application.Mapping;

internal class NurseMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Nurse, NurseProfileResponse>()
            .Map(dest => dest.Name, src => src.User.Name)
            .Map(dest => dest.Email, src => src.User.Email)
            .Map(dest => dest.PhoneNumber, src => src.User.PhoneNumber)
            .Map(dest => dest.City, src => src.User.City)
            .Map(dest => dest.Address, src => src.User.Address)
            .Map(dest => dest.AddressUrl, src => src.User.AddressUrl)
            .Map(dest => dest.Gender, src => src.User.Gender);


        config.NewConfig<UpdateNursePricingCommand, Nurse>()
            .Ignore(dest => dest.UserId);
    }
}
