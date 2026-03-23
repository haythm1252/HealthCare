using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Features.Labs.Contracts;
using HealthCare.Application.Features.LabTest.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Labs.Queries.LabBookingDetails;

public class GetLabBookingDetailsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetLabBookingDetailsQuery, Result<LabBookingDetailsResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<LabBookingDetailsResponse>> Handle(GetLabBookingDetailsQuery request, CancellationToken cancellationToken)
    {
        var response = await _unitOfWork.Labs.AsQueryable()
            .AsNoTracking()
            .AsSplitQuery()
            .Where(l => l.Id == request.Id && !l.User.IsDisabled)
            .Select(l => new LabBookingDetailsResponse(
                l.Id,
                l.User.Name,
                l.Bio,
                l.User.City,
                l.User.Address,
                l.User.AddressUrl,
                l.Rating,
                l.RatingsCount,
                l.HomeVisitFee,
                l.ProfilePictureUrl,
                l.OpeningTime,
                l.ClosingTime,
                l.WorkingDays,

                l.LabTests.Select(lt => new LabTestResponse(
                    lt.Id,
                    lt.Test.Name,
                    lt.Test.Description,
                    lt.Test.PreRequisites,
                    lt.Price,
                    lt.IsAvailableAtHome
                )).ToList()
            ))
            .SingleOrDefaultAsync(cancellationToken);

        if (response is null)
            return Result.Failure<LabBookingDetailsResponse>(LabErrors.NotFound);

        return Result.Success(response);
    }
}
