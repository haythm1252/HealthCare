using FluentValidation;
using HealthCare.Application.Common.Behaviors;
using HealthCare.Application.Common.Settings;
using Mapster;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HealthCare.Application;

public static class ApplicationDependencies
{

    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplicationDependencies(IConfiguration configuration)
        {
            // Register MediatR handlers from this assembly
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(ApplicationDependencies).Assembly);
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            // fluent validation scanning
            services
                .AddValidatorsFromAssembly(typeof(ApplicationDependencies).Assembly);

            // Mapster configuration scanning
            TypeAdapterConfig.GlobalSettings.Scan(typeof(ApplicationDependencies).Assembly);

            // Settings
            services.AddOptions<JwtSettings>()
                .Bind(configuration.GetSection(JwtSettings.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddOptions<CloudinarySettings>()
                .Bind(configuration.GetSection(CloudinarySettings.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddOptions<MailSettings>()
                .Bind(configuration.GetSection(MailSettings.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddOptions<PaymobSettings>()
                .Bind(configuration.GetSection(PaymobSettings.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            return services;
        }
    }
}
