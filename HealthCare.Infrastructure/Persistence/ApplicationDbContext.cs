using HealthCare.Domain.Users;
using HealthCare.Domain.Entities;
using System.Linq;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    //Users
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Nurse> Nurses => Set<Nurse>();
    public DbSet<Lab> Labs => Set<Lab>();

    //Entities
    public DbSet<Chat> Chats => Set<Chat>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Specialty> Specialties => Set<Specialty>();
    public DbSet<LabTest> LabTests => Set<LabTest>();
    public DbSet<DoctorSlot> DoctorSlots => Set<DoctorSlot>();
    public DbSet<NurseAppointment> NurseAppointments => Set<NurseAppointment>();
    public DbSet<DoctorAppointment> DoctorAppointments => Set<DoctorAppointment>();
    public DbSet<LabAppointment> LabAppointments => Set<LabAppointment>();
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Test> Tests => Set<Test>();
    public DbSet<TestResult> TestResults => Set<TestResult>();
    public DbSet<LabShift> LabShifts => Set<LabShift>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<NurseShift> NurseShifts => Set<NurseShift>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        ChangeDeleteBehaviorToRestrict(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    private static void ChangeDeleteBehaviorToRestrict(ModelBuilder modelBuilder)
    {
        var cascadeFKs = modelBuilder.Model
        .GetEntityTypes()
        .SelectMany(x => x.GetForeignKeys())
        .Where(x => x.DeleteBehavior == DeleteBehavior.Cascade && !x.IsOwnership);

        foreach (var fk in cascadeFKs)
        {
            fk.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }
}
