using Hangfire;
using HealthCare.Api;
using HealthCare.Infrastructure;
using HealthCare.Application;
using Serilog;
using HealthCare.Infrastructure.Persistence.Seed;
using HealthCare.Api.Hubs;
using HangfireBasicAuthenticationFilter;
var builder = WebApplication.CreateBuilder(args);


builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services
    .AddApplicationDependencies(builder.Configuration)
    .AddInfrastructureDependencies(builder.Configuration)
    .AddDependencies(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await HealthCareSeeder.SeedDataAsync(services);

   
    services.AddHangfireRecurringJobs();
}


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization =
    [
        new HangfireCustomBasicAuthenticationFilter{
            User = app.Configuration.GetValue<string>("HangfireSettings:User"),
            Pass = app.Configuration.GetValue<string>("HangfireSettings:Pass")
        }
    ]
});

app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseExceptionHandler();
app.MapHub<HealthCareHub>("/healthcare-hub");

try
{
    Log.Information("Starting HealthCare API...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "The application failed to start correctly.");
}
finally
{
    Log.CloseAndFlush();
}
