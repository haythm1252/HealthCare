using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Doctors.Commands.UpdateConsultationSettings;

public class UpdateDoctorConsultationSettingsCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateDoctorConsultationSettingsCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(UpdateDoctorConsultationSettingsCommand request, CancellationToken cancellationToken)
    {
        var doctor = await _unitOfWork.Doctors.AsQueryable()
                    .Where(d => d.UserId == request.UserId)
                    .SingleOrDefaultAsync(cancellationToken);

        if(doctor is null)
            return Result.Failure(UserErrors.NotFound);

        doctor.ClinicFee = request.ClinicFee;
        doctor.OnlineFee = request.OnlineFee;
        doctor.HomeFee = request.HomeFee;       
        doctor.AllowHomeVisit = request.AllowHomeVisit;
        doctor.AllowOnlineConsultation = request.AllowOnlineConsultation;
        doctor.LastModified = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();    
    }
}
