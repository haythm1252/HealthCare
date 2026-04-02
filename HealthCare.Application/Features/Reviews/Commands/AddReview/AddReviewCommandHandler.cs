using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Features.Reviews.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using HealthCare.Domain.Entities;
using HealthCare.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Reviews.Commands.AddReview;

public class AddReviewCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<AddReviewCommand, Result<ReviewResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<ReviewResponse>> Handle(AddReviewCommand request, CancellationToken cancellationToken)
    {
        var patient = await _unitOfWork.Patients.AsQueryable()
            .Where(p => p.UserId == request.UserId)
            .Select(p => new { p.Id, p.User.Name })
            .SingleOrDefaultAsync(cancellationToken);

        if (patient is null)
            return Result.Failure<ReviewResponse>(UserErrors.NotFound);

        var targetType = Enum.Parse<TargetType>(request.TargetType, true);

        var existingReview = await _unitOfWork.Reviews.AsQueryable()
            .Where(r => r.PatientId == patient.Id && r.TargetId == request.TargetId &&
                        r.TargetType == targetType)
            .SingleOrDefaultAsync(cancellationToken);

        bool isReviewExist = existingReview is not null;

        // Update the average rating of the target Doctor, Nurse, or Lab
        switch (targetType)
        {
            case TargetType.Doctor:
                var doctor = await _unitOfWork.Doctors.AsQueryable()
                    .FirstOrDefaultAsync(d => d.Id == request.TargetId, cancellationToken);

                if (doctor is null)
                    return Result.Failure<ReviewResponse>(ReviewErrors.TargetNotFound);

                var hasDocAppointment = await _unitOfWork.DoctorAppointments.AnyAsync(da => da.DoctorId == doctor.Id && da.PatientId == patient.Id, cancellationToken);
                if (!hasDocAppointment)
                    return Result.Failure<ReviewResponse>(ReviewErrors.NoAppointmentExist);

                if (isReviewExist)
                    doctor.Rating = ((doctor.Rating * doctor.RatingsCount) - existingReview!.Rating + request.Rating) / doctor.RatingsCount;
                else
                {
                    doctor.RatingsCount++;
                    doctor.Rating = ((doctor.Rating * doctor.RatingsCount) + request.Rating) / doctor.RatingsCount;
                }
                break;

            case TargetType.Nurse:
                var nurse = await _unitOfWork.Nurses.AsQueryable()
                    .SingleOrDefaultAsync(n => n.Id == request.TargetId, cancellationToken);

                if (nurse is null)
                    return Result.Failure<ReviewResponse>(ReviewErrors.TargetNotFound);

                var hasNurseAppointment = await _unitOfWork.NurseAppointments.AnyAsync(na => na.NurseId == nurse.Id && na.PatientId == patient.Id, cancellationToken);
                if (!hasNurseAppointment)
                    return Result.Failure<ReviewResponse>(ReviewErrors.NoAppointmentExist);

                if (isReviewExist)
                    nurse.Rating = ((nurse.Rating * nurse.RatingsCount) - existingReview!.Rating + request.Rating) / nurse.RatingsCount;
                else
                {
                    nurse.RatingsCount++;
                    nurse.Rating = ((nurse.Rating * nurse.RatingsCount) + request.Rating) / nurse.RatingsCount;
                }

                break;

            case TargetType.Lab:
                var lab = await _unitOfWork.Labs.AsQueryable()
                    .SingleOrDefaultAsync(l => l.Id == request.TargetId, cancellationToken);

                if (lab is null)
                    return Result.Failure<ReviewResponse>(ReviewErrors.TargetNotFound);

                var hasLabAppointment = await _unitOfWork.LabAppointments.AnyAsync(la => la.LabId == lab.Id && la.PatientId == patient.Id, cancellationToken);
                if (!hasLabAppointment)
                    return Result.Failure<ReviewResponse>(ReviewErrors.NoAppointmentExist);

                if (isReviewExist)
                    lab.Rating = ((lab.Rating * lab.RatingsCount) - existingReview!.Rating + request.Rating) / lab.RatingsCount;
                else
                {
                    lab.RatingsCount++;
                    lab.Rating = ((lab.Rating * lab.RatingsCount) + request.Rating) / lab.RatingsCount;
                }

                break;

            default:
                return Result.Failure<ReviewResponse>(ReviewErrors.TargetNotFound);
        }


        Review? finalReview;

        // Save or Update the Review itself
        if (existingReview is not null)
        {
            existingReview.Rating = request.Rating;
            existingReview.Comment = request.Comment;
            existingReview.LastModified = DateTime.UtcNow;
            finalReview = existingReview;
        }
        else
        {
            var newReview = new Review
            {
                PatientId = patient.Id,
                TargetId = request.TargetId,
                TargetType = targetType,
                Rating = request.Rating,
                Comment = request.Comment
            };
            await _unitOfWork.Reviews.AddAsync(newReview, cancellationToken);
            finalReview = newReview;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new ReviewResponse(
            finalReview.Id,
            patient.Name,
            request.Rating,
            request.Comment!,
            DateTime.UtcNow,
            isReviewExist
        );
        return Result.Success(response);
    }
}
