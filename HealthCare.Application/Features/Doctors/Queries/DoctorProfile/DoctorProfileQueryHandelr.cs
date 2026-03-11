using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Features.Doctors.Contracts;
using HealthCare.Application.Features.Nurses.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Doctors.Queries.DoctorProfile
{
    public record DoctorProfileQueryHandelr(IUnitOfWork unitOfWork) : IRequestHandler<DoctorProfileQueries, Result<DoctorProfileResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        public async Task<Result<DoctorProfileResponse>> Handle(DoctorProfileQueries request, CancellationToken cancellationToken)
        {
            var doctor = await _unitOfWork.Doctors.AsQueryable()
                .Where(n => n.UserId == request.UserId)
                .Include(n => n.User)
                .AsNoTracking()
                .SingleOrDefaultAsync(cancellationToken);

            if (doctor == null)
            {
                return Result.Failure<DoctorProfileResponse>(UserErrors.NotFound);
            }

            var doctorProfile = doctor.Adapt<DoctorProfileResponse>();
            return Result.Success(doctorProfile);
        }
    }   
}
