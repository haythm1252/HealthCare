using HealthCare.Application.Common.Consts;
using HealthCare.Domain.Entities;
using HealthCare.Domain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Infrastructure.Persistence.Seed;

public class HealthCareSeeder
{
    public static async Task SeedDataAsync(IServiceProvider service)
    {
        var userManager = service.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = service.GetRequiredService<RoleManager<IdentityRole>>();
        var context = service.GetRequiredService<ApplicationDbContext>();

        // Seed Roles
        await SeedRolesAsync(roleManager);

        // Seed Users
        await SeedAdminAsync(userManager);
        await SeedDoctorAsync(userManager, context);
        await SeedNurseAsync(userManager, context);
        await SeedPatientAsync(userManager, context);
        await SeedLabAsync(userManager, context);
        await SeedTestsAsync(context);
        //await SeedSpecialtiesAsync(context);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        var roles = DefaultRoles.AllRoles;
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    private static async Task SeedAdminAsync(UserManager<ApplicationUser> userManager)
    {
        if (!await userManager.Users.AnyAsync(u => u.Email == DefaultUsers.AdminEmail))
        {
            var admin = new ApplicationUser
            {
                UserName = DefaultUsers.AdminUserName,
                Email = DefaultUsers.AdminEmail,
                EmailConfirmed = true,
                Name = DefaultUsers.AdminName
            };

            await userManager.CreateAsync(admin, DefaultUsers.AdminPassword);
            await userManager.AddToRoleAsync(admin, DefaultRoles.Admin);
        }
    }

    private static async Task SeedDoctorAsync(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        if (!await userManager.Users.AnyAsync(u => u.Email == DefaultUsers.DoctorEmail))
        {
            var user = new ApplicationUser
            {
                UserName = DefaultUsers.DoctorUserName,
                Email = DefaultUsers.DoctorEmail,
                EmailConfirmed = true,
                Name = DefaultUsers.DoctorName
            };

            await userManager.CreateAsync(user, DefaultUsers.DoctorPassword);
            await userManager.AddToRoleAsync(user, DefaultRoles.Doctor);

            var specialty = new Specialty
            {
                Name = "Cardiology"
            };

            context.Specialties.Add(specialty);
            await context.SaveChangesAsync();

            var doctor = new Doctor
            {
                UserId = user.Id,
                SpecialtyId = specialty.Id
            };

            context.Doctors.Add(doctor);

        }
    }

    private static async Task SeedNurseAsync(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        if (!await userManager.Users.AnyAsync(u => u.Email == DefaultUsers.NurseEmail))
        {
            var user = new ApplicationUser
            {
                UserName = DefaultUsers.NurseUserName,
                Email = DefaultUsers.NurseEmail,
                EmailConfirmed = true,
                Name = DefaultUsers.NurseName
            };

            await userManager.CreateAsync(user, DefaultUsers.NursePassword);
            await userManager.AddToRoleAsync(user, DefaultRoles.Nurse);

            var nurse = new Nurse
            {
                UserId = user.Id
            };

            context.Nurses.Add(nurse);
            await context.SaveChangesAsync();
        }
    }

    private static async Task SeedPatientAsync(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        if (!await userManager.Users.AnyAsync(u => u.Email == DefaultUsers.PatientEmail))
        {
            var user = new ApplicationUser
            {
                UserName = DefaultUsers.PatientUserName,
                Email = DefaultUsers.PatientEmail,
                EmailConfirmed = true,
                Name = DefaultUsers.PatientName
            };

            await userManager.CreateAsync(user, DefaultUsers.PatientPassword);
            await userManager.AddToRoleAsync(user, DefaultRoles.Patient);

            var patient = new Patient
            {
                UserId = user.Id
            };

            context.Patients.Add(patient);
            await context.SaveChangesAsync();
        }
    }

    private static async Task SeedLabAsync(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        if (!await userManager.Users.AnyAsync(u => u.Email == DefaultUsers.LabEmail))
        {
            var user = new ApplicationUser
            {
                UserName = DefaultUsers.LabUserName,
                Email = DefaultUsers.LabEmail,
                EmailConfirmed = true,
                Name = DefaultUsers.LabName
            };

            await userManager.CreateAsync(user, DefaultUsers.LabPassword);
            await userManager.AddToRoleAsync(user, DefaultRoles.Lab);

            var lab = new Lab
            {
                UserId = user.Id
            };

            context.Labs.Add(lab);
            await context.SaveChangesAsync();
        }
    }

    //private static async Task SeedSpecialtiesAsync(ApplicationDbContext context)
    //{
    //    var specialties = new List<string>
    //{
    //    "Internal Medicine",
    //    "Cardiology",
    //    "Pulmonology",
    //    "Gastroenterology",
    //    "Nephrology",
    //    "Endocrinology",
    //    "General Surgery",
    //    "Orthopedics",
    //    "Neurosurgery",
    //    "Plastic Surgery",
    //    "Cardiac Surgery",
    //    "Pediatrics",
    //    "Obstetrics and Gynecology",
    //    "Dermatology",
    //    "ENT",
    //    "Ophthalmology",
    //    "Dentistry",
    //    "Neurology",
    //    "Psychiatry",
    //    "Physical Therapy",
    //    "Nutrition",
    //    "Urology",
    //    "Oncology"
    //};

    //    foreach (var name in specialties)
    //    {
    //        if (!await context.Specialties.AnyAsync(s => s.Name == name))
    //        {
    //            context.Specialties.Add(new Specialty
    //            {
    //                Name = name
    //            });
    //        }
    //    }

    //    await context.SaveChangesAsync();
    //}


    private static async Task SeedTestsAsync(ApplicationDbContext context)
    {
        var popularTests = new List<Test>
    {
        new Test
        {
            Name = "Liver Function Test (LFT)",
            Description = "Measures proteins, liver enzymes, and bilirubin in your blood to check liver health.",
            PreRequisites = "Fasting for 8-12 hours is usually recommended."
        },
        new Test
        {
            Name = "Kidney Function Test (KFT)",
            Description = "Includes tests like Creatinine and Urea to evaluate how well your kidneys are working.",
            PreRequisites = "Drink plenty of water before the test."
        },
        new Test
        {
            Name = "Lipid Profile",
            Description = "Comprehensive test for LDL, HDL, and Triglycerides.",
            PreRequisites = "Strict fasting (water only) for 9-12 hours."
        },
        new Test
        {
            Name = "Thyroid Function (TSH)",
            Description = "Measures Thyroid Stimulating Hormone to check for hypo or hyperthyroidism.",
            PreRequisites = "No specific fasting required unless specified by the doctor."
        },
        new Test
        {
            Name = "Hemoglobin A1c (HbA1c)",
            Description = "Measures your average blood sugar levels over the past 3 months.",
            PreRequisites = "No fasting required."
        },
        new Test
        {
            Name = "Vitamin D Test",
            Description = "Measures the level of vitamin D in your blood.",
            PreRequisites = "No special preparation needed."
        },
        new Test
        {
            Name = "Urinalysis",
            Description = "Physical, chemical, and microscopic examination of urine to detect infections.",
            PreRequisites = "The first morning urine sample is preferred."
        }
    };

        foreach (var test in popularTests)
        {
            if (!await context.Tests.AnyAsync(t => t.Name == test.Name))
            {
                context.Tests.Add(test);
            }
        }
        await context.SaveChangesAsync();
    }

}
