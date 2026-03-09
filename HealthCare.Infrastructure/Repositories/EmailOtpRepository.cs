using HealthCare.Application.Interfaces.Repositories;
using HealthCare.Domain.Entities;
using HealthCare.Infrastructure.Persistence;
using HealthCare.Infrastructure.Repositories.Base;

namespace HealthCare.Infrastructure.Repositories;

public class EmailOtpRepository(ApplicationDbContext context) : BaseRepository<EmailOtp>(context), IEmailOtpRepository
{
}
