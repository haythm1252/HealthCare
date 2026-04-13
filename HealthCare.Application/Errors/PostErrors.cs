using HealthCare.Application.Common.Result;

namespace HealthCare.Application.Errors;

public static class PostErrors
{
    public static readonly Error NotFound =
        new("Post.NotFound", "Post not found", 404);

    public static readonly Error Unauthorized =
        new("Post.Unauthorized", "You are not authorized to perform this action", 403);

    public static readonly Error NotFoundOrUnAuthorize =
        new("PostErrors.NotFound", "The Post not found or you dont have the permission", 404);

    public static readonly Error InvalidFile =
        new("Post.InvalidFile", "Invalid file format or size", 400);
    
    public static readonly Error UploadFailed =
        new("Post.UploadFailed", "Failed to upload image to cloud storage", 500);
}
