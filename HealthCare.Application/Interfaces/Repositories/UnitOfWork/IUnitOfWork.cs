using HealthCare.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HealthCare.Application.Interfaces.Repositories.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IPatientRepository Patients { get; }
    IDoctorRepository Doctors { get; }
    INurseRepository Nurses { get; }
    ILabRepository Labs { get; }
    IChatRepository Chats { get; }
    IMessageRepository Messages { get; }
    ISpecialtyRepository Specialties { get; }
    ILabTestRepository LabTests { get; }
    IDoctorSlotRepository DoctorSlots { get; }
    INurseAppointmentRepository NurseAppointments { get; }
    IDoctorAppointmentRepository DoctorAppointments { get; }
    ILabAppointmentRepository LabAppointments { get; }
    IPostRepository Posts { get; }
    ITestRepository Tests { get; }
    ITestResultRepository TestResults { get; }
    IReviewRepository Reviews { get; }
    INurseShiftRepository NurseShifts { get; }
    IEmailOtpRepository EmailOtps { get; }
    IUserRepository Users { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
