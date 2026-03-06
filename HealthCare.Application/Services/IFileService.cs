using HealthCare.Application.Common.Result;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Services;

public interface IFileService
{
    Task<Result<(string Url, string PublicId)>> UploadImageAsync(Stream fileData, string fileName);
    Task<Result> DeleteImageAsync(string publicId);
}
