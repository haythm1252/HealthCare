using HealthCare.Application.Interfaces.Repositories;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using HealthCare.Infrastructure.Persistence;

namespace HealthCare.Infrastructure.Repositories.UnitOfWork;

public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    //lazily initialized to avoid unnecessary instantiation if not used
    private IPatientRepository? _patients;
    private IDoctorRepository? _doctors;
    private INurseRepository? _nurses;
    private ILabRepository? _labs;
    private IChatRepository? _chats;
    private IMessageRepository? _messages;
    private ISpecialtyRepository? _specialties;
    private ILabTestRepository? _labTests;
    private IDoctorSlotRepository? _doctorSlots;
    private INurseAppointmentRepository? _nurseAppointments;
    private IDoctorAppointmentRepository? _doctorAppointments;
    private ILabAppointmentRepository? _labAppointments;
    private IPostRepository? _posts;
    private ITestRepository? _tests;
    private ITestResultRepository? _testResults;
    private IReviewRepository? _reviews;
    private INurseShiftRepository? _nurseShifts;

    // here we create new instances of repositories when they are called for the first time 
    public IPatientRepository Patients => _patients ??= new PatientRepository(context);
    public IDoctorRepository Doctors => _doctors ??= new DoctorRepository(context);
    public INurseRepository Nurses => _nurses ??= new NurseRepository(context);
    public ILabRepository Labs => _labs ??= new LabRepository(context);
    public IChatRepository Chats => _chats ??= new ChatRepository(context);
    public IMessageRepository Messages => _messages ??= new MessageRepository(context);
    public ISpecialtyRepository Specialties => _specialties ??= new SpecialtyRepository(context);
    public ILabTestRepository LabTests => _labTests ??= new LabTestRepository(context);
    public IDoctorSlotRepository DoctorSlots => _doctorSlots ??= new DoctorSlotRepository(context);
    public INurseAppointmentRepository NurseAppointments => _nurseAppointments ??= new NurseAppointmentRepository(context);
    public IDoctorAppointmentRepository DoctorAppointments => _doctorAppointments ??= new DoctorAppointmentRepository(context);
    public ILabAppointmentRepository LabAppointments => _labAppointments ??= new LabAppointmentRepository(context);
    public IPostRepository Posts => _posts ??= new PostRepository(context);
    public ITestRepository Tests => _tests ??= new TestRepository(context);
    public ITestResultRepository TestResults => _testResults ??= new TestResultRepository(context);
    public IReviewRepository Reviews => _reviews ??= new ReviewRepository(context);
    public INurseShiftRepository NurseShifts => _nurseShifts ??= new NurseShiftRepository(context);


    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => context.SaveChangesAsync(cancellationToken);

    public void Dispose() => context.Dispose();
}