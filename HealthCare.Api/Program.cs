using Hangfire;
using HealthCare.Api;
using HealthCare.Api.Middleware;
using HealthCare.Infrastructure;
using HealthCare.Application;
using Microsoft.AspNetCore.Identity;
using Serilog;
using HealthCare.Infrastructure.Persistence.Seed;
var builder = WebApplication.CreateBuilder(args);


builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration)
    //we need to add serilog.slink.file when we go to production
);

builder.Services
    .AddApplicationDependencies(builder.Configuration)
    .AddInfrastructureDependencies(builder.Configuration)
    .AddDependencies(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await HealthCareSeeder.SeedDataAsync(services);
}


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    
    app.UseHangfireDashboard("/hangfire");
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseExceptionHandler();

app.Run();
