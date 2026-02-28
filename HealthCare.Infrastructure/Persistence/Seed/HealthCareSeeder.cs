using HealthCare.Application.Common.Consts;
using HealthCare.Domain.Users;
using Microsoft.AspNetCore.Identity;
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
        var RoleManager = service.GetRequiredService<RoleManager<IdentityRole>>();
        var context = service.GetRequiredService<ApplicationDbContext>();

        await SeedRolesAsync(RoleManager);
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

    //private static async Task SeedPatientAsync(UserManager<ApplicationUser> userManager)
    //{
    //    if (!await userManager.Users.AnyAsync(u => u.Email == DefaultUsers.AdminEmail))
    //    {
    //        var admin = new ApplicationUser
    //        {
    //            UserName = DefaultUsers.PatientEmail,
    //            Email = DefaultUsers.PatientEmail,
    //            EmailConfirmed = true,
    //            Name = DefaultUsers.PatientName
    //        };
    //        await userManager.CreateAsync(admin, DefaultUsers.AdminPassword);
    //        await userManager.AddToRoleAsync(admin, "Admin");
    //    }
    //}
}
