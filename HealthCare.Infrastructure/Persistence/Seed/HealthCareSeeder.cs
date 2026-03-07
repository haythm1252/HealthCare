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

}
