namespace HealthCare.Application.Common.Consts;

public static class FileSettings
{
    public const int MaxImageSizeInMb = 10;
    public const int MaxImageSizeInBytes = MaxImageSizeInMb * 1024 * 1024;

    public const int MaxVideoSizeInMb = 10;
    public const int MaxVideoSizeInBytes = MaxVideoSizeInMb * 1024 * 1024;

    public const int MaxPdfSizeInMb = 10;
    public const int MaxPdfSizeInBytes = MaxPdfSizeInMb * 1024 * 1024;

    public static readonly string[] BlockedSignatures = ["4D-5A", "2F-2A", "D0-CF"];
}
