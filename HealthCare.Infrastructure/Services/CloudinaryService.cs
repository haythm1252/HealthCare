using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using HealthCare.Application.Common.Result;
using HealthCare.Application.Common.Settings;
using HealthCare.Application.Services;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using Error = HealthCare.Application.Common.Result.Error;

namespace HealthCare.Infrastructure.Services;

public class CloudinaryService : ICloudinaryService
{
    private readonly CloudinarySettings _cloudinarySettings;
    private readonly Cloudinary _cloudinary;
    public CloudinaryService(IOptions<CloudinarySettings> cloudinarySettings)
    {
        _cloudinarySettings = cloudinarySettings.Value;
        var account = new Account(
            _cloudinarySettings.CloudName,
            _cloudinarySettings.ApiKey,
            _cloudinarySettings.ApiSecret
        );
        _cloudinary = new Cloudinary(account);
    }

    public async Task<Result<(string Url, string PublicId)>> UploadImageAsync(Stream fileData, string fileName)
    {
        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(fileName, fileData),
            Transformation = new Transformation().Width(500).Height(500).Crop("fill"),
            Folder = "HealthCare/Images",
            UniqueFilename = true
        };
        var result = await _cloudinary.UploadAsync(uploadParams);

        if (result.Error is not null || result.SecureUrl is null)
            return Result.Failure<(string Url, string PublicId)>(new Error("UploadingError", result.Error?.Message ?? "Image upload failed" , 400));

        return Result.Success((result.SecureUrl.ToString(), result.PublicId));
    }

    public async Task<Result> DeleteImageAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId)
        {
            ResourceType = ResourceType.Image 
        };

        var result = await _cloudinary.DestroyAsync(deleteParams);

        if (result.Error is not null)
            return Result.Failure(new Error("DeleteImageError", result.Error.Message, 400));

        if (result.Result == "ok")
            return Result.Success();

        if (result.Result == "not found")
            return Result.Failure(new Error("ImageNotFound", "Image not found.", 404));

        return Result.Failure(new Error("DeleteImageError", "Failed to delete image.", 400));
    }

    public async Task<Result<(string Url, string PublicId)>> UploadPdfAsync(Stream fileData, string fileName)
    {
        var uploadParams = new RawUploadParams()
        {
            File = new FileDescription(fileName, fileData),
            Folder = "HealthCare/TestResults",
            UniqueFilename = true,
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        if (result.Error is not null || result.SecureUrl is null)
            return Result.Failure<(string Url, string PublicId)>(
                new Error("PdfUploadError", result.Error?.Message ?? "PDF upload failed", 400));

        return Result.Success((result.SecureUrl.ToString(), result.PublicId));
    }

    public async Task<Result> DeletePdfAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId)
        {
            ResourceType = ResourceType.Raw
        };

        var result = await _cloudinary.DestroyAsync(deleteParams);

        if (result.Error is not null)
            return Result.Failure(new Error("DeletePdfError", result.Error.Message, 400));

        if (result.Result == "ok")
            return Result.Success();

        if (result.Result == "not found")
            return Result.Failure(new Error("PdfNotFound", "PDF file not found in storage.", 404));

        return Result.Failure(new Error("DeletePdfError", "Failed to delete PDF.", 400));
    }
}
