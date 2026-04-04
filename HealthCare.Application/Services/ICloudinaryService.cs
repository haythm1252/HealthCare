using HealthCare.Application.Common.Result;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Services;

public interface ICloudinaryService
{
    Task<Result<(string Url, string PublicId)>> UploadImageAsync(Stream fileData, string fileName);
    Task<Result> DeleteImageAsync(string publicId);

    Task<Result<(string Url, string PublicId)>> UploadPdfAsync(Stream fileData, string fileName);
    Task<Result> DeletePdfAsync(string publicId);
}
