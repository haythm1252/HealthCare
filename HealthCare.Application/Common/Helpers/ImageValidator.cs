using FluentValidation;
using HealthCare.Application.Common.Consts;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Common.Helpers;

public class ImageValidator : AbstractValidator<IFormFile>
{
    private static readonly List<string> AllowedExtensions = [".jpg", ".jpeg", ".png"];
    private static readonly List<string> AllowedMimeTypes = ["image/jpeg", "image/jpg", "image/png"];

    public ImageValidator()
    {
        RuleFor(file => file.ContentType)
            .Must(HasValidContentType)
            .WithMessage($"File type must be one of: {string.Join(", ", AllowedMimeTypes)}");

        RuleFor(file => file.FileName)
            .Must(HasValidExtension)
            .WithMessage($"Extension must be one of: {string.Join(", ", AllowedExtensions)}");

        RuleFor(file => file.Length)
            .Must(IsWithinSizeLimit)
            .WithMessage($"Image size must be less than {FileSettings.MaxImageSizeInMb}MB.");

    }

    private bool HasValidContentType(string contentType)
        => !string.IsNullOrEmpty(contentType) &&  AllowedMimeTypes.Contains(contentType.ToLower());


    private bool HasValidExtension(string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return false;

        var extension = Path.GetExtension(fileName).ToLower();
        return AllowedExtensions.Contains(extension);
    }

    private bool IsWithinSizeLimit(long fileSize)
        => fileSize <= FileSettings.MaxImageSizeInBytes;

}
